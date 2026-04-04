---
applyTo: src/control-plane/**
---

# DuckHouse Control Plane

ASP.NET Core 10 Web API (.NET Aspire, `DuckHouse.ControlPlane.slnx`) that dynamically provisions and manages compute nodes and Jupyter kernels running on those nodes.

## Node service architecture

`INodeService` has two implementations selected at startup via `NodeService:Backend` in config:

| Backend | Class | "Node" concept |
|---|---|---|
| `Local` | `LocalNodeService` | Kubernetes **pod** in Docker Desktop |
| `Aks` | `AksNodeService` | AKS **agent pool** (one VM, one pod) |

- **Local**: uses `IKubernetes` (reads `~/.kube/config`) to create pods with image `duckhouse-runtime:latest` (`ImagePullPolicy: Never` — local Docker Desktop image). Pods are labelled `app.kubernetes.io/managed-by=duckhouse-control-plane`. Stop/Start are no-ops.
- **AKS**: uses `ArmClient` (Azure SDK) to create AKS agent pools (`Count=1`, `Mode=User`) and `IKubernetes` to deploy one `duckhouse-runtime` pod pinned to that pool via `nodeSelector: kubernetes.azure.com/agentpool: <name>`. The pod runs the image's default entrypoint with no resource caps — it is the sole workload on the node. Pods are deleted before the agent pool is deleted.

**AKS authentication**: Because the cluster has `disableLocalAccounts=true`, `IKubernetes` is not configured from `~/.kube/config`. Instead, `Program.cs` fetches the user kubeconfig from the AKS ARM resource at startup (obtaining the API server URL and CA cert), then replaces the kubelogin exec plugin with `AksTokenProvider` — an `ITokenProvider` that acquires Entra ID tokens from the registered `TokenCredential`. The same credential is shared with `ArmClient`. Authentication falls back to `DefaultAzureCredential` unless `TenantId`/`ClientId`/`ClientSecret` are all set in config (service principal path).

## Kernel runtime client

`INodeRuntimeClient` proxies Jupyter kernel management calls to the `duckhouse-runtime` FastAPI process (port 8000) running inside the node pod. `KubernetesRuntimeClient` tunnels every request through the Kubernetes API server HTTP proxy (`/api/v1/namespaces/default/pods/{pod}:8000/proxy/{path}`) — no Kubernetes Service, public IP, or VNet access required.

Supported operations: List / Create / Get / Delete kernels, Execute code, Restart kernel, Interrupt kernel.

## Key files

```
Nodes/
  INodeService.cs                    — interface (List/Create/Remove/Stop/Start)
  NodeInfo.cs                        — record returned by INodeService
  NodeState.cs                       — enum (Unknown/Stopped/Resuming/Running/Stopping/Deleting/Creating/Failure)
  NodeEndpoints.cs                   — minimal API routes: /nodes and /nodes/{name}/kernels/*
  CreateNodeRequest.cs               — request record (Name, VmSize?)
  Local/LocalNodeService.cs
  Aks/AksNodeService.cs
  Aks/AksNodeOptions.cs              — Section "NodeService:Aks": RuntimeImage, DefaultVmSize, NodeSubnetId, SP credentials
  Aks/AksTokenProvider.cs            — bridges Azure.Core TokenCredential → k8s ITokenProvider
  Kernels/INodeRuntimeClient.cs      — interface for kernel operations
  Kernels/KubernetesRuntimeClient.cs — K8s API server HTTP proxy implementation
  Kernels/KernelModels.cs            — KernelInfo, ExecuteRequest, ExecutionResult, Output, ErrorInfo
Program.cs                           — backend selection and DI wiring
```

## Infrastructure (src/infra)

Bicep deploys AKS cluster, VNet, and ACR together (`main.bicep` → `modules/aks.bicep`):
- Two user-assigned managed identities: one for the cluster control plane, one as the kubelet identity (shared by all node pools).
- The kubelet identity has `AcrPull` on the ACR — no image pull secrets needed in pod specs.
- A VNet with a dedicated node subnet is provisioned; the cluster identity gets `Network Contributor` on the subnet.
- `disableLocalAccounts: true` and Azure RBAC are enabled on the cluster.
- The API principal (`apiPrincipalId` parameter) receives three role assignments on the cluster: **AKS Contributor** (node pool management), **AKS Cluster User** (fetch kubeconfig), **AKS RBAC Cluster Admin** (Kubernetes data-plane access for pods and kernel proxy).
- Deployment outputs: `acrLoginServer` → set `NodeService:Aks:RuntimeImage` (e.g. `<acrLoginServer>/duckhouse-runtime:latest`); `nodeSubnetId` → set `NodeService:Aks:NodeSubnetId`.
