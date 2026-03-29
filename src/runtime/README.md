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