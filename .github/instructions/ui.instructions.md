---
applyTo: src/ui/**
---

# DuckHouse UI

Blazor Web App with a server host and an interactive WebAssembly client. Uses Bootstrap (compiled from SCSS with SASS), BlazorBootstrap for ready-made components, and Lucide icons via a custom Roslyn source generator.

The solution is split into three top-level folders: `server/`, `client/`, and `DuckHouse.Ui.Shared/`.

## Project structure

### Shared
| Project | SDK / Target | Role |
|---|---|---|
| `DuckHouse.Ui.Shared` | `Microsoft.NET.Sdk` / net10.0 | Shared type definitions between server and client (e.g. API request/response models) |

### Server (`server/`)

The server follows Clean Architecture with four layers:

| Project | SDK / Target | Role |
|---|---|---|
| `DuckHouse.Ui.Server` | `Microsoft.NET.Sdk.Web` / net10.0 | ASP.NET Core host; serves static assets, bootstraps WASM, wires DI |
| `DuckHouse.Ui.Server.Application` | `Microsoft.NET.Sdk` / net10.0 | Use-case layer; mediator pattern, commands, queries, `AddApplicationServices()` |
| `DuckHouse.Ui.Server.Core` | `Microsoft.NET.Sdk` / net10.0 | Domain layer; repository interfaces (no implementations) |
| `DuckHouse.Ui.Server.Infrastructure` | `Microsoft.NET.Sdk` / net10.0 | Infrastructure layer; concrete repository implementations, `AddInfrastructureServices()` |

### Client (`client/`)

| Project | SDK / Target | Role |
|---|---|---|
| `DuckHouse.Ui.Client` | `Microsoft.NET.Sdk.BlazorWebAssembly` / net10.0 | Interactive WASM client; pages, layouts, routing |
| `DuckHouse.Ui.Client.Components` | `Microsoft.NET.Sdk.Razor` / browser | Razor Class Library; shared components, icon generation target |
| `DuckHouse.Ui.Client.Icons` | `Microsoft.NET.Sdk` / netstandard2.0 | `GenerateIconsAttribute` and `Svg` wrapper — no dependencies |
| `DuckHouse.Ui.Client.SourceGeneration` | `Microsoft.NET.Sdk` / netstandard2.0 | Roslyn `IIncrementalGenerator`; generates typed icon properties at compile time |

## Hosting model

`DuckHouse.Ui.Server` is the HTTP host. Its `App.razor` renders `<Routes>` and `<HeadOutlet>` with `@rendermode="InteractiveWebAssembly"`. All interactive pages live in `DuckHouse.Ui.Client`. Routing is defined in `DuckHouse.Ui.Client/Routes.razor` using `<Router AppAssembly="typeof(Program).Assembly">`.

## Server Clean Architecture

- **Core** — defines repository contracts (e.g. `INodeRepository`, `IKernelRepository`). No implementation details.
- **Application** — use cases implemented via a custom mediator pattern (`IMediator`, `IRequest<T>`, `IRequestHandler<TReq, TRes>`, `MediatorImpl`). Commands and queries live in `Mediator/Commands/` and `Mediator/Queries/`. Register with `services.AddApplicationServices()`.
- **Infrastructure** — implements Core interfaces (e.g. `NodeRepository : INodeRepository`). Register with `services.AddInfrastructureServices()`.
- **Server host** — `Program.cs` calls both service extension methods and bootstraps the Blazor+WASM pipeline.

## Shared project

`DuckHouse.Ui.Shared` is intended for types shared between the server REST API and the WASM client — e.g. API request/response DTOs. Both `DuckHouse.Ui.Server` and `DuckHouse.Ui.Client` reference it.

## Styling — Bootstrap + SASS

The app stylesheet is generated from SCSS. **Never edit `bootstrap.custom.css` directly** — always edit the SCSS source and recompile.

- **Source**: `server/DuckHouse.Ui.Server/wwwroot/css/bootstrap.custom.scss`
  - Imports Bootstrap from `../lib/bootstrap/scss/bootstrap`
  - Contains Lucide icon sizing rules (`.lucide`, `.lucide-sm`) and `.btn .lucide` alignment tweaks
  - Contains Blazor error UI (`#blazor-error-ui`) positioning
- **Compiled output**: `server/DuckHouse.Ui.Server/wwwroot/css/bootstrap.custom.css`
- **Compile command** (from the `css/` folder):
  ```
  sass bootstrap.custom.scss:bootstrap.custom.css
  ```

## BlazorBootstrap

`Blazor.Bootstrap` is referenced from `DuckHouse.Ui.Client`. It is registered in `DuckHouse.Ui.Client/Program.cs`:

```csharp
builder.Services.AddBlazorBootstrap();
```

Its CSS and JS are loaded from `_content/Blazor.Bootstrap/` in `App.razor`. Use BlazorBootstrap components (e.g. `<Button>`, `<Modal>`, `<Grid>`) in WASM client pages and components.

## Lucide icon source generation

Icons are Lucide SVG files. At compile time the source generator reads the SVG files and generates a static `partial class` with one property per icon. **Do not add icons by hand** — drop `.svg` files into the `Icons/lucide/` folder in `DuckHouse.Ui.Client.Components` and rebuild.

### How it works

1. **`DuckHouse.Ui.Client.Icons`** defines two types:
   - `GenerateIconsAttribute(string? cssClass, params string[] iconsLocationPathSegments)` — marks a `public static partial class` for generation.
   - `Svg(string svg)` — wraps raw SVG markup.

2. **`DuckHouse.Ui.Client.SourceGeneration`** contains `IconSourceGenerator : IIncrementalGenerator`:
   - Finds classes annotated with `[GenerateIcons]` (must be `public static partial`).
   - Reads `.svg` files registered as `<AdditionalFiles>` whose path contains the declared path segments.
   - Injects the `cssClass` into each SVG's `class` attribute (merging if already present).
   - Emits `{ClassName}.g.cs` with a property per icon; property names are the kebab-case filename converted to PascalCase.

3. **`DuckHouse.Ui.Client.Components`** is the consumer. `DuckHouse.Ui.Client.Icons` and `DuckHouse.Ui.Client.SourceGeneration` are both referenced as `OutputItemType="Analyzer"`. The `Icons/lucide/` folder is declared as `<AdditionalFiles>`.

4. **`LucideIcon`** is the generated class:
   ```csharp
   [GenerateIcons(cssClass: "lucide", "Icons", "lucide")]
   public static partial class LucideIcon;
   // Generated:
   // public static Svg Info => new Svg("<svg class=\"lucide\" .../>"); 
   // public static Svg OctagonX => new Svg("<svg class=\"lucide\" .../>"); 
   ```

5. **`SvgIcon`** is a Blazor component that renders an `Svg` value:
   ```razor
   <SvgIcon Icon="LucideIcon.Info" />
   ```

## Key files

```
src/ui/
  DuckHouse.Ui.Shared/
    DuckHouse.Ui.Shared.csproj         — shared API models (server ↔ client)
  server/
    DuckHouse.Ui.Server/
      Program.cs                       — server startup; MapRazorComponents, MapStaticAssets
      Components/
        App.razor                      — HTML shell; loads Bootstrap CSS/JS, BlazorBootstrap assets
        Pages/Error.razor              — server-side error page
      wwwroot/css/
        bootstrap.custom.scss          — SCSS source (edit this)
        bootstrap.custom.css           — compiled output (do not edit)
    DuckHouse.Ui.Server.Core/
      Repositories/
        INodeRepository.cs             — node repository interface
        IKernelRepository.cs           — kernel repository interface
    DuckHouse.Ui.Server.Application/
      ServiceExtensions.cs             — AddApplicationServices()
      Mediator/
        IMediator.cs / IRequest.cs / IRequestHandler.cs / MediatorImpl.cs
        Commands/CreateNode.cs         — CreateNodeRequest + handler
        Queries/GetNodes.cs            — GetNodesRequest + handler
    DuckHouse.Ui.Server.Infrastructure/
      ServiceExtensions.cs             — AddInfrastructureServices()
      Repositories/NodeRepository.cs   — INodeRepository implementation
  client/
    DuckHouse.Ui.Client/
      Program.cs                       — AddBlazorBootstrap(), WebAssemblyHostBuilder
      Routes.razor                     — client-side router
      Layout/MainLayout.razor          — default layout
      Pages/
        Home.razor                     — home page
        NotFound.razor                 — 404 page
    DuckHouse.Ui.Client.Components/
      LucideIcon.cs                    — [GenerateIcons] partial class (generation target)
      SvgIcon.cs                       — ComponentBase that renders Svg.Text as markup
      Icons/lucide/                    — Lucide SVG source files (AdditionalFiles for generator)
      wwwroot/defaults.css             — component-scoped CSS
    DuckHouse.Ui.Client.Icons/
      GenerateIconsAttribute.cs        — attribute definition
      Svg.cs                           — SVG wrapper record
    DuckHouse.Ui.Client.SourceGeneration/
      IconSourceGenerator.cs           — IIncrementalGenerator implementation
      Extensions.cs                    — kebab-case → PascalCase helper
      IconData.cs                      — SVG file path + text
      IconsClassData.cs                — decorated class metadata
      DiagnosticDescriptors.cs         — ICON001: incorrect class modifiers
```
