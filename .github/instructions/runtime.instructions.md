---
applyTo: src/runtime/**
---

# DuckHouse Runtime

FastAPI service (`duckhouse_runtime` package, `pyproject.toml`) that runs on each Kubernetes node and manages Jupyter Python kernels. The control plane's `KubernetesRuntimeClient` communicates with it via the Kubernetes API server HTTP proxy on port 8000.

## Architecture

Two completely isolated Python environments live in the Docker image:

| Environment | Path                | Contents                                                                     |
| ----------- | ------------------- | ---------------------------------------------------------------------------- |
| API         | `/opt/venvs/api`    | FastAPI, uvicorn, pydantic, jupyter-client, ipykernel, jedi, pyflakes        |
| Kernel      | `/opt/venvs/kernel` | ipykernel, duckdb (+ any extras from `KERNEL_PACKAGES` or requirements file) |

`DUCKHOUSE_KERNEL_PYTHON=/opt/venvs/kernel/bin/python` tells the kernel manager which executable to launch. This keeps API and kernel dependencies fully separate.

**Kernel lifecycle**: `KernelRegistry` (module-level singleton `registry`) owns all running kernels. On FastAPI shutdown (`lifespan`) it calls `registry.shutdown_all()` to clean up. Each `KernelConnection` wraps a `jupyter_client` `AsyncKernelManager` + `AsyncKernelClient`. Executions are serialised per kernel via an `asyncio.Lock` but run in a background asyncio Task — `start_execute()` returns an `execution_id` immediately and `get_execution()` is used to poll for completion. Output is collected from iopub messages until `status: idle` is received for the relevant `msg_id`.

**Kernel statuses**: `starting` → `idle` ↔ `busy`; `restarting` during restart; `dead` after shutdown.

**DataFrame formatter**: After a kernel starts (or restarts), `_setup_formatters()` sends the source of `dataframe_formatter.py` to the kernel as a silent execute. This registers a custom IPython MIME formatter for `application/vnd.duckhouse.dataframe+json` that handles `pandas.DataFrame` and `duckdb.DuckDBPyRelation` objects, producing structured JSON with columns, rows (capped at 10 000), and row counts.

**Code intelligence**: `complete()` uses Jedi (pointed at the kernel Python environment) to return completions aware of prior-cell context. `diagnose()` uses pyflakes and Python's `compile()` for syntax and lint diagnostics, also context-aware.

## Key files

```
duckhouse_runtime/
  main.py                  — FastAPI app, lifespan (shutdown_all), uvicorn entrypoint
  api/
    health.py              — GET /health → {"status": "ok"}
    kernels.py             — APIRouter at /kernels; all kernel HTTP endpoints
  kernels/
    manager.py             — KernelConnection, KernelRegistry, _DuckhouseKernelManager
    models.py              — Pydantic models: KernelInfo, ExecutionHandle, ExecuteRequest,
                             ExecutionResult, PollExecutionResponse, Output, ErrorInfo,
                             CompleteRequest, CompleteResponse, CompletionItem,
                             DiagnoseRequest, DiagnoseResponse, Diagnostic
    dataframe_formatter.py — IPython MIME formatter for DataFrames/DuckDB relations;
                             injected into each kernel on start/restart
Dockerfile                 — two-venv image; copies built wheel from dist/
docker/
  entrypoint.sh            — installs optional packages into kernel venv, then starts API
pyproject.toml             — package metadata and dependencies; entry point: duckhouse-runtime
```

## API endpoints

All endpoints are under `/kernels`:

| Method   | Path                                      | Description                                                                      |
| -------- | ----------------------------------------- | -------------------------------------------------------------------------------- |
| `POST`   | `/kernels`                                | Create a new kernel (starts the subprocess, waits for ready)                     |
| `GET`    | `/kernels`                                | List all kernels                                                                 |
| `GET`    | `/kernels/{id}`                           | Get a single kernel                                                              |
| `DELETE` | `/kernels/{id}`                           | Shut down and remove a kernel                                                    |
| `POST`   | `/kernels/{id}/execute`                   | Start async execution; body: `{code, timeout?}`; returns `ExecutionHandle` (202) |
| `GET`    | `/kernels/{id}/executions/{execution_id}` | Poll execution result; returns `PollExecutionResponse`                           |
| `POST`   | `/kernels/{id}/restart`                   | Restart the kernel process                                                       |
| `POST`   | `/kernels/{id}/interrupt`                 | Send an interrupt signal                                                         |
| `POST`   | `/kernels/{id}/completions`               | Jedi completions; body: `{code, line, column, context?}`                         |
| `POST`   | `/kernels/{id}/diagnostics`               | pyflakes/syntax diagnostics; body: `{code, context?}`                            |

Error cases: 404 if kernel (or execution) not found, 408 on execution timeout, 409 if kernel is dead.

**Execution is async/poll**: `POST .../execute` enqueues the execution in a background asyncio Task and returns `{"execution_id": "..."}` immediately (HTTP 202). The caller must poll `GET .../executions/{execution_id}` until `is_complete` is `true`. `timeout` in `ExecuteRequest` is optional (`None` = no timeout).

## Environment variables

| Variable                  | Default          | Description                                                         |
| ------------------------- | ---------------- | ------------------------------------------------------------------- |
| `DUCKHOUSE_KERNEL_PYTHON` | `sys.executable` | Python executable for kernel processes                              |
| `KERNEL_PACKAGES`         | _(none)_         | Space-separated package names installed into kernel venv at startup |

A requirements file mounted at `/etc/duckhouse/kernel-requirements.txt` is also installed into the kernel venv at startup (before `KERNEL_PACKAGES`).

## Docker image

Build requires the wheel to be built first (`py -m build --wheel`), then:

```
docker build -t duckhouse-runtime .        # from src/runtime/
docker run --rm -p 8000:8000 duckhouse-runtime
```

Interactive docs available at `http://localhost:8000/docs`. Push to ACR with `az acr login` + `docker tag` + `docker push`; AKS pulls without credentials because the kubelet identity has `AcrPull` (granted by the Bicep template).
