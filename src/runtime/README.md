## DuckHouse Runtime

Fast API runtime for DuckHouse. The API runs on each node in the Kubernetes cluster and manages the Python kernels.

### Development and build environment setup

Create virtual environment in the workspace root.

```
> py -m venv .venv
```

Activate virtual environment.

```
> .\.venv\Scripts\activate
```

Install dependencies defined in `pyproject.toml` using pip.

```
> pip install .
```

Install `build` using pip.

```
> pip install build
```

Build the wheel package with `build`. **Note, that the build and target (installation) environments should be using the same Python version.**

```
> py -m build --wheel
```

The wheel package can then be installed in development mode.

```
pip install --editable .
```

### Installation (Linux)

First make sure both python3 and pip are installed.

```
$ sudo apt install python3
$ sudo apt install python3-pip
```

Then install pipx and make sure relevant PATH additions are in place (`ensurepath`).

```
$ sudo apt install pipx
$ pipx ensurepath
```

After that, you can install the built .whl (wheel) file.

```
pipx install ./<file_name>.whl
```

### Installation (Windows)

First, make sure Python and pip are installed. Use the installer available from the Python website.

Install pipx and make sure relevant PATH additions are in place.

```
> py -m pip install --upgrade pipx
> py -m pipx ensurepath
```

Reopen the terminal for PATH changes to take effect in the terminal session.

After that, you can install the built .whl (wheel) file.

```
> py -m pipx install ./<file_name>.whl
```

The DuckHouse runtime package depends on the duckdb Python package, which in turn requires [Microsoft Visual C++ Redistributable](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#latest-supported-redistributable-version) to be installed on the machine (Windows). See [this GitHub issue](https://github.com/duckdb/duckdb/issues/8101) for more context. Otherwise the runtime will fail during execution.

### Docker

The container image runs the API in an isolated Python environment and spins up kernels in a separate Python environment. Only `ipykernel` is required in the kernel environment; the API dependencies (FastAPI, uvicorn, etc.) are not available to kernels and vice versa.

#### Build the image

Build the wheel first (see [Development and build environment setup](#development-and-build-environment-setup)), then build the Docker image from the `src/runtime` directory.

```
docker build -t duckhouse-runtime .
```

The kernel environment comes with `ipykernel`, `duckdb`, `numpy` and `pandas` pre-installed. Additional packages can be supplied at runtime without rebuilding (see below).

#### Configuring kernel packages without rebuilding

Packages can also be installed in the kernel environment at container startup, without rebuilding the image. This is useful when different Kubernetes deployments need different packages but should share the same base image.

**Via environment variable** — set `KERNEL_PACKAGES` in the pod spec or `docker run` command.

```
docker run --rm -p 8000:8000 -e KERNEL_PACKAGES="numpy pandas" duckhouse-runtime
```

**Via a mounted requirements file** — mount a `requirements.txt` to `/etc/duckhouse/kernel-requirements.txt`. In Kubernetes this is typically done with a ConfigMap.

```yaml
# configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: kernel-requirements
data:
  requirements.txt: |
    requests
    
```

```yaml
# deployment.yaml (relevant excerpt)
volumeMounts:
  - name: kernel-requirements
    mountPath: /etc/duckhouse
volumes:
  - name: kernel-requirements
    configMap:
      name: kernel-requirements
      items:
        - key: requirements.txt
          path: kernel-requirements.txt
```

Both mechanisms can be used together. The requirements file is processed first, followed by `KERNEL_PACKAGES`. Note that installing packages on startup adds to the container's startup time.

#### Run locally with Docker Desktop

After building, the image is immediately available in Docker Desktop. Run it locally to verify.

```
docker run --rm -p 8000:8000 duckhouse-runtime
```

The API will be available at `http://localhost:8000`. Interactive API docs are at `http://localhost:8000/docs`.

#### Publish to Azure Container Registry (ACR)

Log in to your ACR instance. Replace `<registry>` with your registry name.

```
az acr login --name <registry>
```

Tag the local image with the fully qualified ACR repository name.

```
docker tag duckhouse-runtime <registry>.azurecr.io/duckhouse-runtime:<tag>
```

For example:

```
docker tag duckhouse-runtime acrduckhousedev.azurecr.io/duckhouse-runtime:latest
```

Push the image.

```
docker push <registry>.azurecr.io/duckhouse-runtime:<tag>
```

Reference the image in a Kubernetes pod spec or Helm chart.

```yaml
image: <registry>.azurecr.io/duckhouse-runtime:<tag>
```

Make sure the Kubernetes nodes have pull access to the registry. For AKS this is typically done by attaching the ACR to the cluster. This is automatically done by the Bicep template in the infra folder of this repository.

```
az aks update --name <aks-cluster> --resource-group <resource-group> --attach-acr <registry>
```