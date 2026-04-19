# DuckHouse UI

Blazor Web App: an ASP.NET Core server host (`DuckHouse.Ui.Server`) with a WebAssembly client (`DuckHouse.Ui.Client`). Provides the notebook editor, SQL query editor, workspace browser, catalog management, and interactive node pool management.

---

## Project Structure

| Project                              | Role                                                               |
| ------------------------------------ | ------------------------------------------------------------------ |
| `DuckHouse.Ui.Shared`                | DTOs shared between server and WASM client                         |
| `DuckHouse.Ui.Server`                | ASP.NET Core host; REST API controllers; EF Core migrations        |
| `DuckHouse.Ui.Server.Core`           | Domain entities and repository interfaces                          |
| `DuckHouse.Ui.Server.Application`    | Use-case layer: custom mediator pattern (commands + queries)       |
| `DuckHouse.Ui.Server.Infrastructure` | EF Core + SQLite; concrete repositories and services               |
| `DuckHouse.Ui.Client`                | WASM client: pages and typed `HttpClient` services                 |
| `DuckHouse.Ui.Client.Components`     | Razor Class Library: `CodeCell` (Monaco wrapper), `ExecutionTimer` |

---

## DuckLake Catalogs

DuckLake catalogs are instances of the [DuckLake](https://ducklake.select/) open table format backed by a Postgres metadata store and a parquet data store (local path or Azure Data Lake Storage). The UI allows users to create, browse, and manage catalogs in the `/catalogs` page.

### Managed vs External catalogs

**Managed** catalogs use a shared Postgres instance and base data path configured in `appsettings`. Their connection details are not stored in the database — they are populated at runtime from `CatalogSettings`. This means Postgres credentials and the data path can be updated by changing appsettings without touching the database.

**External** (unmanaged) catalogs have all connection details filled in by the user at creation time. Their details (including encrypted password and connection string) are stored in the database.

### Object explorer

The workspace sidebar shows a catalog tree. Catalogs expand to show schemas; schemas expand to show type folders (Tables, Views, Functions, etc.); folders expand to show individual objects; tables and views expand to show columns and their types.

### Automatic catalog attachment

When a notebook or SQL query is opened, the user can select catalogs to attach to the kernel session. The UI generates a Python setup script that:

1. Installs and loads the `ducklake` extension (and the `azure` extension when needed).
2. Creates DuckDB secrets for the Postgres catalog store and Azure storage.
3. Executes `ATTACH 'ducklake:postgres:' AS catalog_name (DATA_PATH '...')`.

All SQL is sent to the kernel as Python using `duckdb.execute("...")`.

### Configuration

| Key                                | Default                         | Description                                                                                                                                |
| ---------------------------------- | ------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| `Catalogs:BaseDataPath`            | _(none)_                        | Base path for managed catalog parquet data. Final path = `BaseDataPath/catalogName`. Can be a local path or `abfss://` for Azure Data Lake |
| `Catalogs:StorageConnectionString` | _(none)_                        | Azure Data Lake connection string. Required when `BaseDataPath` uses `az://` or `abfss://`                                                 |
| `Catalogs:CatalogHost`             | `localhost`                     | Postgres host for **direct connections** from the UI server process (admin ops: CREATE DATABASE, metadata queries)                         |
| `Catalogs:CatalogPodHost`          | _(falls back to `CatalogHost`)_ | Postgres host as seen **from inside kernel pods**. Set this when the address differs from `CatalogHost`                                    |
| `Catalogs:CatalogPort`             | `5432`                          | Postgres port                                                                                                                              |
| `Catalogs:CatalogUser`             | _(none)_                        | Postgres user                                                                                                                              |
| `Catalogs:CatalogPassword`         | _(none)_                        | Postgres password                                                                                                                          |

### Local development setup

During local development the UI server runs on the developer machine and can reach Postgres via `localhost`. Kernel pods run in Docker Desktop Kubernetes and must use `host.docker.internal` instead. Configure both addresses:

```json
// appsettings.Development.json
"Catalogs": {
  "BaseDataPath": "/data/ducklake",
  "CatalogHost": "localhost",
  "CatalogPodHost": "host.docker.internal",
  "CatalogPort": 5432,
  "CatalogUser": "postgres",
  "CatalogPassword": "..."
}
```

`BaseDataPath` here matches the `DataVolumeMountPath` set in the Control Plane configuration so that parquet files survive pod restarts. See the [Control Plane README](../control-plane/README.md#persistent-data-volume-local-development) for instructions on mounting a host directory into pods.

---

## Pages

| Page | Route | Description |
|---|---|---|
| `Home.razor` | `/` | Welcome page |
| `WorkspacePage.razor` | `/workspace` | Workspace browser: create, rename, move, clone, delete notebooks and queries |
| `NotebookPage.razor` | `/notebook`, `/notebook/{id}` | Polyglot notebook editor; connects to an interactive node pool |
| `QueryPage.razor` | `/query`, `/query/{id}` | SQL editor with results panel; connects to an interactive node pool |
| `NodePoolsPage.razor` | `/node-pools` | Node pool config management with separate tabs for Interactive and Job pools |
| `Nodes.razor` | `/nodes` | Active nodes monitoring (admin view; admins can manually remove nodes) |
| `Kernels.razor` | `/nodes/{name}/kernels` | Kernel management per node |
| `KernelSession.razor` | `/nodes/{name}/kernels/{id}` | Interactive kernel REPL |
| `CatalogsPage.razor` | `/catalogs` | DuckLake catalog management |
| `Settings.razor` | `/settings` | Theme settings |

---

## Styling

Custom CSS in `DuckHouse.Ui.Server/wwwroot/css/app.css`. Component-scoped CSS in `DuckHouse.Ui.Client.Components/wwwroot/defaults.css`. Dark mode class `ant-dark` is toggled on `<html>`.

Do not use `var(--ant-*)` CSS custom properties — they do not exist in this build of Ant Design Blazor. Use hardcoded hex values and target dark mode with `html.ant-dark` selectors in `app.css`. Typical values: borders `#d9d9d9` / `#434343` (dark), subtle backgrounds `#fafafa` / `#1d1d1d` (dark).
