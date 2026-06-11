# Datateal Repository Agent Instructions

This repository contains the source code for **Datateal**, an open-source lakehouse analytics platform. Datateal provides a self-hosted, cloud-agnostic environment for interactive data exploration (polyglot notebooks and SQL editors) and automated data pipelines.

---

## Architecture Overview

Datateal is composed of several architectural modules:

1. **UI (`src/ui`)**
   - **Role**: Blazor Web App (ASP.NET Core host `Datateal.Ui.Server` + WebAssembly client `Datateal.Ui.Client`).
   - **Key Features**: Workspace browser (notebooks/SQL queries), polyglot notebook editor (Python, SQL, Markdown cells), standalone SQL editor, node pool management (interactive & job pools).
   - **Tech Stack**: Ant Design Blazor, BlazorMonaco (Monaco Editor), Markdig.

2. **Control Plane (`src/control-plane`)**
   - **Role**: ASP.NET Core 10 Web API (`Datateal.ControlPlane`) that dynamically provisions compute nodes and manages Jupyter kernels running on them.
   - **Key Features**: Local node backend (Kubernetes pod in Docker Desktop) and AKS node backend (ARM AKS agent pools).
   - **Proxy**: Tunnels kernel execution traffic through the Kubernetes API server HTTP proxy.

3. **Orchestrator (`src/orchestrator`)**
   - **Role**: ASP.NET Core Web API that schedules and executes multi-task jobs (notebook tasks, SQL query tasks, sub-job tasks) against compute nodes managed by the Control Plane.
   - **Key Features**: Eager skip propagation DAG engine, Quartz.NET cron scheduler, warm standby job node pools, crash recovery.

4. **Runtime (`src/runtime`)**
   - **Role**: FastAPI service (`datateal_runtime` package) running inside compute pods that manages Jupyter Python kernels.
   - **Key Features**: Separate API/Kernel virtual environments, custom DataFrame formatting for DuckDB/Pandas, Jedi-powered code completions, and pyflakes-powered code diagnostics.

5. **Shared (`src/shared`)**
   - Contains domain model definitions (e.g., `NodeInfo`, `KernelInfo`), the primary database context (`DatatealDbContext` for SQLite/PostgreSQL), migrations and auth libraries.

6. **Infrastructure (`src/infra`)**
   - Contains Bicep templates for deploying Azure resources (AKS, ACR, VNets, and managed identities).

---

## Global Technical Conventions

- **Clean Architecture**: Backend C# services follow a clean architecture pattern (Core → Application → Infrastructure → Host).
- **Mediator Pattern**: Decoupled internal communication is implemented via a custom Mediator pattern (`IMediator`, commands, queries).
- **Database Model**: UI Server and Orchestrator share the same PostgreSQL database (`datateal-ui` resource in Aspire).
- **Node & Kernel Lifecycle**: Compute nodes can be either "Interactive" (1 stable node on-demand, evicted after inactivity timeout) or "Job" pools (provisioned per job, optionally using warm standby nodes).
- **Jupyter Kernels**: Always one kernel per task or notebook session; kernels are never shared between tasks.

---

## Agent Instructions & Repository Guidelines

- **Module-Specific Guidance**: When working on a specific module, you MUST refer to and follow the instructions in the `AGENTS.md` file in that module's directory (e.g. `src/ui/AGENTS.md`).
- **Thin Wrappers**: Agent-specific configuration files (`CLAUDE.md` and `GEMINI.md`) act as thin wrappers pointing to the local `AGENTS.md` file in their respective directories.
- **Maintain Clean Codebases**: Follow the established structural, typing, and architectural patterns of each module. Preserve all comments and docstrings.
