---
applyTo: src/orchestrator/**
---

# DuckHouse Orchestrator

ASP.NET Core Web API that schedules and executes multi-task jobs against compute nodes managed by the Control Plane. Follows the same Clean Architecture and custom mediator pattern as the UI server and Control Plane.

## Projects

| Project                                 | Role                                                                   |
| --------------------------------------- | ---------------------------------------------------------------------- |
| `DuckHouse.Orchestrator`                | ASP.NET Core host; minimal API endpoints                               |
| `DuckHouse.Orchestrator.Application`    | Engine services, mediator handlers, YAML import/export, DAG validation |
| `DuckHouse.Orchestrator.Core`           | Entity classes, enums, repository and infrastructure interfaces        |
| `DuckHouse.Orchestrator.Infrastructure` | EF Core repositories; `ControlPlaneClient` HTTP wrapper                |

Shared domain types (`NodeInfo`, `KernelInfo`, mediator interfaces) live in `src/shared/DuckHouse.Core`. The orchestrator shares the same PostgreSQL database as the UI server — Aspire resource name `duckhouse-ui`. There is no separate database.

## Engine service lifetime model

`RunDispatcher` is a **singleton** because it must track every in-flight run across DI scopes (one dictionary entry per active run). `RunCoordinator` and `TaskExecutor` are **scoped** — `RunDispatcher` creates a fresh DI scope for each run so they get their own EF Core `DbContext`. Never make `RunCoordinator` a singleton or inject a scoped service directly into `RunDispatcher`.

## Job snapshot

When a run is triggered, `TriggerJobHandler` serialises the complete `Job` entity (tasks, parameters, dependencies) as `SnapshotJson` on the `JobRun`. `RunCoordinator` executes from this snapshot, not the live database. This means:

- Editing a job never affects a run that is already in progress or recovering from a crash.
- When adding fields to `Job` or its children, consider whether the snapshot serialisation needs updating (it uses `ReferenceHandler.IgnoreCycles` and `DefaultIgnoreCondition.WhenWritingNull`).

## DAG execution

The DAG loop in `RunCoordinator.RunDagAsync` runs a fixed-point **skip propagation** pass before every dispatch cycle, then collects all tasks whose dependency conditions are satisfied and launches them concurrently via `Task.Run`. The loop terminates when there are no active tasks and no ready tasks remaining.

**Skip propagation is eager**: a task is skipped as soon as any single dependency edge becomes permanently unsatisfiable (e.g., `OnSuccess` on an upstream that `Failed`). Downstream tasks that depended on the skipped task are then re-evaluated in the next propagation iteration.

**Retries reset in-place**: on failure, if attempts remain, the same `TaskRun` row has its status set to `Retrying`, then — after the configured `RetryInterval` — its `AttemptNumber` is incremented and status reset to `Pending`. A new `TaskRun` row is never created for a retry.

**Outcome rule**: the run succeeds only if every task ended `Succeeded` or `Skipped`. Any `Cancelled` task → `Cancelled` run. Anything else → `Failed`.

## Node and kernel management

**One node per `NodePoolRef` per run.** `NodeManager` provisions a node on the first `EnsureNodeAsync` call for a given pool ref and returns the cached name on every subsequent call. Tasks that share the same `NodePoolRef` therefore share the same node — by design, to avoid redundant provisioning costs.

**Interactive vs Job pool behaviour in `NodeManager`.** The pool type determines how node allocation works:

- **`JobNodePoolConfig`** — a run-scoped node is created with a deterministic name `j` + 11 hex chars from SHA-256 of `(runId, poolRef)`. The node is marked `Provisioned = true` and deleted by `CleanupAllAsync` when the run finishes.
- **`InteractiveNodePoolConfig`** — the deterministic pool node name (`i` + 11 hex chars of the pool GUID, from `GetNodeName()`) is used. If the node is already Running, the task joins it immediately. Otherwise it is created (with 409 race handling) and polled until Running. The node is marked `Provisioned = false` so `CleanupAllAsync` **does not delete it** — the inactivity eviction service handles teardown.

**One kernel per task.** `TaskExecutor` creates a kernel at the start of each notebook/SQL task and deletes it in the `finally` block, even on failure. Kernels are not shared between tasks. This ensures no Python state leaks between tasks.

Node names:
- Job pool nodes: `j` + 11 lowercase hex chars (SHA-256 of runId + poolRef). Unique per (run, pool) pair.
- Interactive pool nodes: `i` + first 11 hex chars of the pool GUID (from `InteractiveNodePoolConfig.GetNodeName()`). Stable across pool renames; max 12 characters.

The 63-character Kubernetes name limit applies. Keep `NodePoolRef` names short (≤ 50 characters for job pools).

Node cleanup (`CleanupAllAsync`) happens in `RunCoordinator`'s `finally` block using `CancellationToken.None` so it runs even after cancellation. Only nodes with `Provisioned = true` (job pool nodes) are deleted; interactive pool nodes are left running.

## Node pool config types

`NodePoolConfig` is **abstract** with TPH in EF Core (discriminator `PoolType varchar(32)`). The two concrete types are:

- **`JobNodePoolConfig`** (PoolType = `"Job"`) — used by the orchestrator for run-scoped nodes. A new node is created per pool ref per run and deleted when the run ends.
- **`InteractiveNodePoolConfig`** (PoolType = `"Interactive"`) — used by interactive sessions from NotebookPage and QueryPage. Has a stable K8s node name derived from the pool GUID (`GetNodeName()`). The node is created on demand and deleted by the inactivity eviction service — not when a job run ends.

When a job task references an interactive pool, `NodeManager` joins the existing running node (or creates one if idle). The node is **not** stopped when the job completes — this mirrors interactive user behaviour.

`YamlJobImporter` always creates `JobNodePoolConfig` rows for inline `nodePools` entries in YAML (job pools are the only appropriate pool type for YAML-defined pipelines).

- **`NotebookTask`** — executes a workspace notebook cell-by-cell on a kernel. Requires `NodePoolRef`.
- **`SqlQueryTask`** — executes a workspace SQL query file on a kernel, wrapped as `import duckdb; duckdb.sql("""...""")`. Requires `NodePoolRef`.
- **`SubJobTask`** — triggers another job and polls until it completes. No kernel or node involved. The child run is linked via `ParentRunId` / `ParentTaskRunId`.

When adding a new task type: add it to `JobTask`'s `[JsonDerivedType]` attributes, add a case to `TaskExecutor.ExecuteAsync`, and add a corresponding branch in `TriggerJobHandler` (where `TaskType` string is assigned) and in `YamlJobImporter`.

## Notebook execution details

SQL cells in a notebook are automatically wrapped as DuckDB Python calls — the kernel environment only runs Python. The `%run path/to/notebook` magic inlines another notebook's code cells recursively (depth limit 10, cycle detection via a `HashSet<Guid>`). `%run` works with both notebooks and SQL query files; it is expanded before the cell is sent to the kernel.

Parameter injection: if the notebook has a cell tagged `parameters` and the task supplies parameters, an `injected-parameters` cell is inserted immediately after it before execution. The injected cell is never stored back into the notebook source.

## Recovery

`RecoveryService` runs once on startup and re-dispatches any `JobRun` found with status `Running` or `Pending`. A re-dispatched run loads its snapshot and resumes the DAG — `TaskRun` rows that are already in a terminal state are not re-executed (the DAG loop only dispatches `Pending` tasks). This means the recovery path is the same code path as normal execution; no special recovery logic exists beyond the re-dispatch.

## Scheduling

`SchedulerService` reloads all enabled `JobSchedule` rows from the database every evaluation cycle. Schedule changes (add / update / delete) take effect without restarting the service. `NextFireTime` is tracked per schedule so the service is safe to restart — it will not re-fire a schedule that already fired in the current interval.

Supports both 5-field (standard) and 6-field (with seconds) cron expressions via the `Cronos` library.

## YAML import

`YamlJobImporter` resolves notebook and query **paths** to IDs via `IWorkspaceReader`, and sub-job **names** to IDs via `IJobRepository`, at import time. IDs are stored; paths/names are not. If a workspace item is later moved or renamed, the stored ID still resolves correctly. The DAG is validated with Kahn's algorithm before the job is persisted.

Inline `nodePools` entries in the YAML create `NodePoolConfig` rows if they do not already exist (matched by name). Existing configs are not updated on import.

## Adding new features

- **New mediator command/query**: add the request record and handler to `Application/Mediator/Commands/` or `Queries/`. The mediator scans by assembly marker (`ScanEntryPoint` in `ServiceExtensions.cs`).
- **New endpoint**: add a static class in `DuckHouse.Orchestrator/Endpoints/` and register it in `Program.cs`.
- **New entity / EF migration**: the orchestrator uses the same `DuckHouseDbContext` (in `src/shared`) as the UI server. Migrations live in the UI server project. Run `dotnet ef migrations add` from `DuckHouse.Ui.Server`.
