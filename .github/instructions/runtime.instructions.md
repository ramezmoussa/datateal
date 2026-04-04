---
applyTo: src/runtime/**
---

# DuckHouse Runtime

FastAPI service (`duckhouse_runtime` package, `pyproject.toml`) that runs on each Kubernetes node and manages Jupyter Python kernels. The control plane's `KubernetesRuntimeClient` communicates with it via the Kubernetes API server HTTP proxy on port 8000.

## Architecture

Two completely isolated Python environments live in the Docker image:

| Environment | Path | Contents |
|---|---|---|
| API | `/opt/venvs/api` | FastAPI, uvicorn, pydantic, jupyter-client, ipykernel |
| Kernel | `/opt/venvs/kernel` | ipykernel, duckdb (+ any extras from `KERNEL_PACKAGES` or requirements file) |

`DUCKHOUSE_KERNEL_PYTHON=/opt/venvs/kernel/bin/python` tells the kernel manager which executable to launch. This keeps API and kernel dependencies fully separate.

**Kernel lifecycle**: `KernelRegistry` (module-level singleton `registry`) owns all running kernels. On FastAPI shutdown (`lifespan`) it calls `registry.shutdown_all()` to clean up. Each `KernelConnection` wraps a `jupyter_client` `AsyncKernelManager` + `AsyncKernelClient`. Execution collects iopub messages until a `status: idle` message is received for the relevant `msg_id`, with a configurable timeout (default 60 s). Executions are serialised per kernel via an `asyncio.Lock`.

**Kernel statuses**: `starting` → `idle` ↔ `busy`; `restarting` during restart; `dead` after shutdown.

## Key files

```
duckhouse_runtime/
  main.py                  — FastAPI app, lifespan (shutdown_all), uvicorn entrypoint
  api/
    health.py              — GET /health → {"status": "ok"}
    kernels.py             — APIRouter at /kernels; all kernel HTTP endpoints
  kernels/
    manager.py             — KernelConnection, KernelRegistry, _DuckhouseKernelManager
    models.py              — Pydantic models: KernelInfo, ExecuteRequest, ExecutionResult, Output, ErrorInfo
Dockerfile                 — two-venv image; copies built wheel from dist/
docker/
  entrypoint.sh            — installs optional packages into kernel venv, then starts API
pyproject.toml             — package metadata and dependencies; entry point: duckhouse-runtime
```

## API endpoints

All endpoints are under `/kernels`:

| Method | Path | Description |
|---|---|---|
| `POST` | `/kernels` | Create a new kernel (starts the subprocess, waits for ready) |
| `GET` | `/kernels` | List all kernels |
| `GET` | `/kernels/{id}` | Get a single kernel |
| `DELETE` | `/kernels/{id}` | Shut down and remove a kernel |
| `POST` | `/kernels/{id}/execute` | Execute code; body: `{code, timeout?}`; returns `ExecutionResult` |
| `POST` | `/kernels/{id}/restart` | Restart the kernel process |
| `POST` | `/kernels/{id}/interrupt` | Send an interrupt signal |

Error cases: 404 if kernel not found, 408 on execution timeout, 409 if kernel is dead.

## Environment variables

| Variable | Default | Description |
|---|---|---|
| `DUCKHOUSE_KERNEL_PYTHON` | `sys.executable` | Python executable for kernel processes |
| `KERNEL_PACKAGES` | _(none)_ | Space-separated package names installed into kernel venv at startup |

A requirements file mounted at `/etc/duckhouse/kernel-requirements.txt` is also installed into the kernel venv at startup (before `KERNEL_PACKAGES`).

## Docker image

Build requires the wheel to be built first (`py -m build --wheel`), then:

```
docker build -t duckhouse-runtime .        # from src/runtime/
docker run --rm -p 8000:8000 duckhouse-runtime
```

Interactive docs available at `http://localhost:8000/docs`. Push to ACR with `az acr login` + `docker tag` + `docker push`; AKS pulls without credentials because the kubelet identity has `AcrPull` (granted by the Bicep template).
