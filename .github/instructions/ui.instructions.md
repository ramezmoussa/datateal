---
applyTo: src/ui/**
---

# DuckHouse UI

Blazor Web App with a server host and an interactive WebAssembly client. Uses **Ant Design Blazor** (`AntDesign`) for UI components, **BlazorMonaco** for code editing, and **Markdig** for Markdown rendering.

The solution is split into three top-level folders: `server/`, `client/`, and `DuckHouse.Ui.Shared/`.

## Project structure

### Shared

| Project               | SDK / Target                  | Role                                                                                                     |
| --------------------- | ----------------------------- | -------------------------------------------------------------------------------------------------------- |
| `DuckHouse.Ui.Shared` | `Microsoft.NET.Sdk` / net10.0 | Shared request/response DTOs between server API and WASM client. Has `Nodes/` and `Kernels/` subfolders. |

### Server (`server/`)

The server follows Clean Architecture with four layers:

| Project                              | SDK / Target                      | Role                                                                                             |
| ------------------------------------ | --------------------------------- | ------------------------------------------------------------------------------------------------ |
| `DuckHouse.Ui.Server`                | `Microsoft.NET.Sdk.Web` / net10.0 | ASP.NET Core host; serves static assets, bootstraps WASM, exposes REST API controllers, wires DI |
| `DuckHouse.Ui.Server.Application`    | `Microsoft.NET.Sdk` / net10.0     | Use-case layer; mediator pattern, commands, queries, `AddApplicationServices()`                  |
| `DuckHouse.Ui.Server.Core`           | `Microsoft.NET.Sdk` / net10.0     | Domain layer; repository interfaces (no implementations)                                         |
| `DuckHouse.Ui.Server.Infrastructure` | `Microsoft.NET.Sdk` / net10.0     | Infrastructure layer; concrete repository implementations, `AddInfrastructureServices()`         |

### Client (`client/`)

| Project                          | SDK / Target                                    | Role                                                                  |
| -------------------------------- | ----------------------------------------------- | --------------------------------------------------------------------- |
| `DuckHouse.Ui.Client`            | `Microsoft.NET.Sdk.BlazorWebAssembly` / net10.0 | Interactive WASM client; pages, layouts, routing, HTTP services       |
| `DuckHouse.Ui.Client.Components` | `Microsoft.NET.Sdk.Razor` / browser             | Razor Class Library; shared components (`CodeCell`, `ExecutionTimer`) |

## Hosting model

`DuckHouse.Ui.Server` is the HTTP host. Its `App.razor` renders `<Routes>` with `new InteractiveWebAssemblyRenderMode(prerender: false)`. All interactive pages live in `DuckHouse.Ui.Client`. Routing is defined in `DuckHouse.Ui.Client/Routes.razor`.

The server also integrates with **.NET Aspire**: `Program.cs` calls `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`.

## Server Clean Architecture

- **Core** — defines repository contracts (`INodeRepository`, `IKernelRepository`). No implementation details.
- **Application** — use cases implemented via a custom mediator pattern (`IMediator`, `IRequest<T>`, `IRequestHandler<TReq, TRes>`, `MediatorImpl`). Commands and queries live in `Mediator/Commands/` and `Mediator/Queries/`. Register with `services.AddApplicationServices()`.
- **Infrastructure** — implements Core interfaces (`NodeRepository`, `KernelRepository`). Register with `services.AddInfrastructureServices()`.
- **Server host** — `Program.cs` calls both service extension methods, registers MVC controllers (`AddControllers()`), and bootstraps the Blazor+WASM pipeline.

### REST API controllers

The server exposes a REST API consumed by the WASM client:

| Controller          | Route                          | Key endpoints                                                                                                                                                                                                   |
| ------------------- | ------------------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `NodesController`   | `api/nodes`                    | GET (list), GET `/{name}`, POST (create), DELETE `/{name}`, POST `/{name}/stop`, POST `/{name}/start`                                                                                                           |
| `KernelsController` | `api/nodes/{nodeName}/kernels` | GET (list), POST (create), GET `/{id}`, DELETE `/{id}`, POST `/{id}/execute`, GET `/{id}/executions/{execId}`, POST `/{id}/restart`, POST `/{id}/interrupt`, POST `/{id}/completions`, POST `/{id}/diagnostics` |

Execution is **asynchronous and poll-based**: `POST .../execute` returns an `ExecutionHandle` (with `ExecutionId`); the client polls `GET .../executions/{executionId}` for a `PollExecutionResponse` until `IsComplete` is true.

### Mediator commands and queries

**Commands** (`Mediator/Commands/`): `CreateNode`, `RemoveNode`, `StopNode`, `StartNode`, `CreateKernel`, `DeleteKernel`, `ExecuteKernel`, `CompleteKernel`, `DiagnoseKernel`, `InterruptKernel`, `RestartKernel`

**Queries** (`Mediator/Queries/`): `GetNodes`, `GetNode`, `GetKernels`, `GetKernel`, `PollExecution`

## Shared project

`DuckHouse.Ui.Shared` contains DTOs shared between the server REST API and the WASM client:

- `Nodes/CreateNodeRequest.cs`
- `Kernels/ExecuteKernelRequest.cs`

## Styling

The app uses Ant Design Blazor's CSS. Custom styles go in `server/DuckHouse.Ui.Server/wwwroot/css/app.css`.

Assets loaded in `App.razor`:

- `_content/AntDesign/css/ant-design-blazor.css` (or `.dark.css` for dark mode, switched via JS)
- `css/app.css`
- `_content/DuckHouse.Ui.Components/defaults.css`

## Ant Design Blazor

`AntDesign` is referenced from both `DuckHouse.Ui.Server` and `DuckHouse.Ui.Client`. It is registered in both `Program.cs` files:

```csharp
builder.Services.AddAntDesign();
```

Its JS is loaded from `_content/AntDesign/js/ant-design-blazor.js` in `App.razor`. Use Ant Design components (e.g. `<Button>`, `<Table>`, `<Drawer>`, `<Alert>`, `<Tag>`, `<Flex>`, `<Space>`, `<Icon>`) in WASM client pages and components.

For icons, use `<Icon Type="@IconType.Outline.X" />` where `X` is an Ant Design icon name.

## Theme system

Dark/light/auto theming is handled by `IThemeService` / `ThemeService` in `DuckHouse.Ui.Client/Services/`. The `AppTheme` enum has `Auto`, `Light`, and `Dark` values.

Theme is persisted in `localStorage` as `duckhouse-theme`. JS interop functions are defined in `App.razor`:

- `setDuckhouseTheme(theme)` — switches Ant Design CSS and Monaco editor theme
- `getStoredDuckhouseTheme()` — reads stored preference
- `getDuckhouseMonacoTheme()` — returns `"vs"` or `"vs-dark"` for Monaco

A FOUC-prevention script in `<head>` applies the theme before page paint by swapping the Ant Design CSS `href`.

## Client services

The WASM client communicates with the server REST API via typed `HttpClient` services registered in `DuckHouse.Ui.Client/Program.cs`:

| Interface        | Implementation  | Base path                      |
| ---------------- | --------------- | ------------------------------ |
| `INodeService`   | `NodeService`   | `api/nodes`                    |
| `IKernelService` | `KernelService` | `api/nodes/{nodeName}/kernels` |
| `IThemeService`  | `ThemeService`  | (JS interop only)              |

## Notebook feature

`NotebookPage.razor` (`/notebook`) is a full notebook UI. It owns a `NotebookDocument` (title + list of `NotebookCell`s) and manages kernel connectivity.

### Notebook model (`DuckHouse.Ui.Client/Notebook/`)

- `NotebookDocument` — title, list of cells, dirty flag
- `NotebookCell` — cell type (`Code`/`Markdown`), language (`Python`/`Sql`), source, outputs, error, execution count, duration, UI state (IsExecuting, ExecutionId, IsEditingMarkdown)
- `NotebookSerializer` — serialises/deserialises to/from `.ipynb` JSON format

### Notebook UI components

- `NotebookCellView.razor` — renders a single cell; delegates to `KernelCodeCell` or markdown view
- `KernelCodeCell.razor` — code cell wrapper with run/interrupt/delete/move toolbar
- `AddCellBar.razor` — `+` bar between cells; lets user add Python, SQL, or Markdown cell
- `DataFrameView.razor` — renders tabular output from kernel results
- `CodeCell.razor` (in `DuckHouse.Ui.Client.Components`) — Monaco editor wrapper; auto-sizes, supports `Ctrl+Enter` to execute, language switching via JS interop
- `ExecutionTimer.razor` (in `DuckHouse.Ui.Client.Components`) — live elapsed timer while a cell is running

SQL cells are wrapped as `import duckdb; duckdb.sql("""...""")` before sending to the kernel.

## Client pages

| Page                  | Route                              | Description                               |
| --------------------- | ---------------------------------- | ----------------------------------------- |
| `Home.razor`          | `/`                                | Welcome page                              |
| `Nodes.razor`         | `/nodes`                           | List, create, start/stop/remove nodes     |
| `Kernels.razor`       | `/nodes/{Name}/kernels`            | List and manage kernels on a node         |
| `KernelSession.razor` | `/nodes/{Name}/kernels/{KernelId}` | Interactive kernel REPL session           |
| `NotebookPage.razor`  | `/notebook`                        | Notebook editor with open/save (`.ipynb`) |
| `Settings.razor`      | `/settings`                        | App settings (theme toggle, etc.)         |
| `NotFound.razor`      | —                                  | 404 page                                  |

## Monaco editor JS interop

`App.razor` loads BlazorMonaco's JS (`_content/BlazorMonaco/...`). Additional helpers defined there:

- `setMonacoEditorLanguage(editorId, language)` — switches language on an existing editor model
- `getDuckhouseMonacoTheme()` — used by `CodeCell` to pick the correct theme on init
- `openFileAsText(inputElement)` — reads a file input as text (used by notebook open)
- `downloadFile(filename, content)` — triggers a file download (used by notebook save)
- `clickElement(element)` — programmatically clicks an element (used to open the file picker)

## Key files

```
src/ui/
  DuckHouse.Ui.Shared/
    Nodes/CreateNodeRequest.cs         — shared node creation DTO
    Kernels/ExecuteKernelRequest.cs    — shared kernel execution DTO
  server/
    DuckHouse.Ui.Server/
      Program.cs                       — server startup; Aspire, AddControllers, MapRazorComponents
      Components/App.razor             — HTML shell; loads AntDesign CSS/JS, BlazorMonaco JS
      Controllers/
        NodesController.cs             — REST API: api/nodes
        KernelsController.cs           — REST API: api/nodes/{nodeName}/kernels
      wwwroot/css/app.css              — custom styles
    DuckHouse.Ui.Server.Core/
      Repositories/
        INodeRepository.cs             — node repository interface
        IKernelRepository.cs           — kernel repository interface
    DuckHouse.Ui.Server.Application/
      ServiceExtensions.cs             — AddApplicationServices()
      Mediator/
        IMediator.cs / IRequest.cs / IRequestHandler.cs / MediatorImpl.cs
        Commands/                      — CreateNode, RemoveNode, StopNode, StartNode,
                                         CreateKernel, DeleteKernel, ExecuteKernel,
                                         CompleteKernel, DiagnoseKernel, InterruptKernel, RestartKernel
        Queries/                       — GetNodes, GetNode, GetKernels, GetKernel, PollExecution
    DuckHouse.Ui.Server.Infrastructure/
      ServiceExtensions.cs             — AddInfrastructureServices()
      Repositories/
        NodeRepository.cs              — INodeRepository implementation
        KernelRepository.cs            — IKernelRepository implementation
  client/
    DuckHouse.Ui.Client/
      Program.cs                       — AddAntDesign(), typed HttpClient services
      Routes.razor                     — client-side router
      Layout/MainLayout.razor          — default layout with NavMenu
      Pages/
        Home.razor                     — home page
        Nodes.razor                    — node management
        Kernels.razor                  — kernel management per node
        KernelSession.razor            — interactive kernel REPL
        NotebookPage.razor             — notebook editor (/notebook)
        Settings.razor                 — theme and app settings
        NotFound.razor                 — 404 page
      Notebook/
        NotebookDocument.cs            — document model (title, cells, dirty flag)
        NotebookCell.cs                — cell model + NotebookCellType/Language enums
        NotebookSerializer.cs          — .ipynb JSON serialisation
      Services/
        INodeService.cs / NodeService.cs     — REST client for api/nodes
        IKernelService.cs / KernelService.cs — REST client for api/nodes/{n}/kernels
        IThemeService.cs / ThemeService.cs   — theme management via JS interop
      AddCellBar.razor                 — + bar to add cells between existing cells
      DataFrameView.razor              — tabular output renderer
      KernelCodeCell.razor             — code cell with toolbar
      NotebookCellView.razor           — cell container (code or markdown)
    DuckHouse.Ui.Client.Components/
      CodeCell.razor                   — Monaco editor wrapper component
      ExecutionTimer.razor             — live elapsed time display
      wwwroot/defaults.css             — component-scoped CSS
```
