---
applyTo: src/control-plane/**
---

# DuckHouse Control Plane

ASP.NET Core 10 Web API (.NET Aspire, `DuckHouse.ControlPlane.slnx`) that dynamically provisions and manages compute nodes and Jupyter kernels running on those nodes.

## Project structure (Clean Architecture)

| Project                                 | Role                                                                                                            |
| --------------------------------------- | --------------------------------------------------------------------------------------------------------------- |
| `DuckHouse.ControlPlane`                | ASP.NET Core host; minimal API endpoints, Aspire wiring                                                         |
| `DuckHouse.ControlPlane.Application`    | Use-case layer; mediator handlers, `InactivityEvictionService`, `AddApplicationServices()`                      |
| `DuckHouse.ControlPlane.Core`           | Domain interfaces: `INodeService`, `INodeRuntimeClient`                                                         |
| `DuckHouse.ControlPlane.Infrastructure` | Implementations: `LocalNodeService`, `AksNodeService`, `KubernetesRuntimeClient`, `AddInfrastructureServices()` |

Domain types (`NodeInfo`, `NodeState`, `KernelInfo`, `KernelStatus`, kernel request/response models, mediator interfaces) live in the **shared** project `src/shared/DuckHouse.Core`, which is also referenced by the UI server.

## Node service architecture

`INodeService` has two implementations selected at startup via `NodeService:Backend` in config:

| Backend | Class              | "Node" concept                       | Config section      |
| ------- | ------------------ | ------------------------------------ | ------------------- |
| `Local` | `LocalNodeService` | Kubernetes **pod** in Docker Desktop | `NodeService:Local` |
| `Aks`   | `AksNodeService`   | AKS **agent pool** (one VM, one pod) | `NodeService:Aks`   |

- **Local**: uses `IKubernetes` (reads `~/.kube/config`, context from `LocalNodeOptions.KubeContext`, default `"docker-desktop"`) to create pods with image `duckhouse-runtime:latest` (`ImagePullPolicy: Never`). Pods are labelled `app.kubernetes.io/managed-by=duckhouse-control-plane`. Stop/Start are no-ops.
- **AKS**: uses `ArmClient` (Azure SDK) to create AKS agent pools (`Count=1`, `Mode=User`) and `IKubernetes` to deploy one `duckhouse-runtime` pod pinned to that pool via `nodeSelector: kubernetes.azure.com/agentpool: <name>`. The pod runs the image's default entrypoint with no resource caps. Pods are deleted before the agent pool is deleted.

**AKS authentication**: Because the cluster has `disableLocalAccounts=true`, `IKubernetes` is not configured from `~/.kube/config`. Instead, `AddInfrastructureServices()` fetches the user kubeconfig from the AKS ARM resource (obtaining the API server URL and CA cert), then replaces the kubelogin exec plugin with `AksTokenProvider` — an `ITokenProvider` that acquires Entra ID tokens from the registered `TokenCredential`. The same credential is shared with `ArmClient`. Authentication falls back to `DefaultAzureCredential` unless `TenantId`/`ClientId`/`ClientSecret` are all set in config (service principal path).

**`AksNodeOptions`** (`NodeService:Aks`): `SubscriptionId`, `ResourceGroupName`, `ClusterName`, `RuntimeImage`, `DefaultVmSize`, `NodeSubnetId`, `TenantId`, `ClientId`, `ClientSecret`.

## Kernel runtime client

`INodeRuntimeClient` proxies all kernel operations to the `duckhouse-runtime` FastAPI process (port 8000) inside the node pod. `KubernetesRuntimeClient` tunnels every request through the Kubernetes API server HTTP proxy (`/api/v1/namespaces/default/pods/{pod}:8000/proxy/{path}`) — no Kubernetes Service, public IP, or VNet access required.

Supported operations: List / Create / Get / Delete kernels; StartExecute / PollExecution; Restart / Interrupt kernel; Complete / Diagnose.

## Inactivity eviction

`InactivityEvictionService` (registered as a `BackgroundService` by `AddApplicationServices()`) periodically sweeps all running nodes:

1. **Phase 1 — kernel eviction**: deletes any `Idle` kernel whose `LastActivity` exceeds `KernelIdleTimeout` (default 10 min).
2. **Phase 2 — node stop**: if a node has no kernels remaining and its last observed kernel activity exceeds `NodeIdleTimeout` (default 20 min), the node is stopped.

Configured via the `InactivityEviction` config section:

| Key                 | Default    | Description                    |
| ------------------- | ---------- | ------------------------------ |
| `Enabled`           | `true`     | Enable/disable the sweep       |
| `KernelIdleTimeout` | `00:10:00` | Idle kernel eviction threshold |
| `NodeIdleTimeout`   | `00:20:00` | Empty node stop threshold      |
| `CheckInterval`     | `00:01:00` | How often the sweep runs       |

## API endpoints

Minimal API routes registered by `NodeEndpoints.MapNodeEndpoints()`:

| Method   | Path                                                        | Description                                      |
| -------- | ----------------------------------------------------------- | ------------------------------------------------ |
| `GET`    | `/nodes`                                                    | List all nodes                                   |
| `GET`    | `/nodes/{name}`                                             | Get a single node                                |
| `POST`   | `/nodes`                                                    | Create a node                                    |
| `DELETE` | `/nodes/{name}`                                             | Remove a node                                    |
| `POST`   | `/nodes/{name}/stop`                                        | Stop a node                                      |
| `POST`   | `/nodes/{name}/start`                                       | Start a node                                     |
| `GET`    | `/nodes/{name}/kernels`                                     | List kernels on a node                           |
| `POST`   | `/nodes/{name}/kernels`                                     | Create a kernel on a node                        |
| `GET`    | `/nodes/{name}/kernels/{kernelId}`                          | Get a kernel                                     |
| `DELETE` | `/nodes/{name}/kernels/{kernelId}`                          | Delete a kernel                                  |
| `POST`   | `/nodes/{name}/kernels/{kernelId}/execute`                  | Start execution (202, returns `ExecutionHandle`) |
| `GET`    | `/nodes/{name}/kernels/{kernelId}/executions/{executionId}` | Poll execution result                            |
| `POST`   | `/nodes/{name}/kernels/{kernelId}/restart`                  | Restart a kernel                                 |
| `POST`   | `/nodes/{name}/kernels/{kernelId}/interrupt`                | Interrupt a kernel                               |
| `POST`   | `/nodes/{name}/kernels/{kernelId}/completions`              | Jedi code completions                            |
| `POST`   | `/nodes/{name}/kernels/{kernelId}/diagnostics`              | pyflakes/syntax diagnostics                      |

## Key files

```
src/shared/DuckHouse.Core/           — shared domain types (referenced by control plane + UI)
  Nodes/NodeInfo.cs                  — NodeInfo record, NodeState enum, CreateNodeRequest
  Kernels/                           — KernelInfo, KernelStatus, ExecuteRequest, ExecutionHandle,
                                       ExecutionResult, PollExecutionResponse, Output, ErrorInfo,
                                       CompleteRequest/Response, CompletionItem, DiagnoseRequest/Response, Diagnostic
  Mediator/                          — IMediator, IRequest<T>, IRequestHandler<TReq,TRes>, MediatorImpl, MediatorExtensions

src/control-plane/
  DuckHouse.ControlPlane/
    Program.cs                       — Aspire, AddApplicationServices, AddInfrastructureServices, MapNodeEndpoints
    Endpoints/NodeEndpoints.cs       — all minimal API routes
  DuckHouse.ControlPlane.Core/
    Services/INodeService.cs         — List/Create/Get/Remove/Stop/Start
    Services/INodeRuntimeClient.cs   — kernel proxy interface (StartExecute, PollExecution, Complete, Diagnose, …)
  DuckHouse.ControlPlane.Application/
    ServiceExtensions.cs             — AddApplicationServices(); registers mediator + InactivityEvictionService
    Mediator/Commands/               — CreateNode, RemoveNode, StopNode, StartNode,
                                       CreateKernel, DeleteKernel, ExecuteKernel,
                                       RestartKernel, InterruptKernel, CompleteKernel, DiagnoseKernel
    Mediator/Queries/                — GetNodes, GetNode, GetKernels, GetKernel, PollExecution
    InactivityEviction/
      InactivityEvictionOptions.cs   — Enabled, KernelIdleTimeout, NodeIdleTimeout, CheckInterval
      InactivityEvictionService.cs   — BackgroundService; sweeps kernels and nodes
  DuckHouse.ControlPlane.Infrastructure/
    ServiceExtensions.cs             — AddInfrastructureServices(); selects backend, wires IKubernetes
    Nodes/Local/LocalNodeService.cs
    Nodes/Local/LocalNodeOptions.cs  — Section "NodeService:Local": KubeContext
    Nodes/Aks/AksNodeService.cs
    Nodes/Aks/AksNodeOptions.cs      — Section "NodeService:Aks": SubscriptionId, ResourceGroupName,
                                       ClusterName, RuntimeImage, DefaultVmSize, NodeSubnetId, SP credentials
    Nodes/Aks/AksTokenProvider.cs    — bridges Azure.Core TokenCredential → k8s ITokenProvider
    Nodes/Kernels/KubernetesRuntimeClient.cs — K8s API server HTTP proxy implementation
```

## Infrastructure (src/infra)

Bicep deploys AKS cluster, VNet, and ACR together (`main.bicep` → `modules/aks.bicep`):

- Two user-assigned managed identities: one for the cluster control plane, one as the kubelet identity (shared by all node pools).
- The kubelet identity has `AcrPull` on the ACR — no image pull secrets needed in pod specs.
- A VNet with a dedicated node subnet is provisioned; the cluster identity gets `Network Contributor` on the subnet.
- `disableLocalAccounts: true` and Azure RBAC are enabled on the cluster.
- The API principal (`apiPrincipalId` parameter) receives three role assignments on the cluster: **AKS Contributor** (node pool management), **AKS Cluster User** (fetch kubeconfig), **AKS RBAC Cluster Admin** (Kubernetes data-plane access for pods and kernel proxy).
- Deployment outputs: `acrLoginServer` → set `NodeService:Aks:RuntimeImage` (e.g. `<acrLoginServer>/duckhouse-runtime:latest`); `nodeSubnetId` → set `NodeService:Aks:NodeSubnetId`.
