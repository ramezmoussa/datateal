---
applyTo: src/control-plane/**
---

# DuckHouse Control Plane

ASP.NET Core 10 Web API (.NET Aspire, `DuckHouse.ControlPlane.slnx`) that dynamically provisions and manages compute nodes.

## Node service architecture

`INodeService` has two implementations selected at startup via `NodeService:Backend` in config:

| Backend | Class | "Node" concept |
|---|---|---|
| `Local` | `LocalNodeService` | Kubernetes **pod** in Docker Desktop |
| `Aks` | `AksNodeService` | AKS **agent pool** (one VM, one pod) |

- **Local**: uses `IKubernetes` to create pods with image `duckhouse-runtime:latest` (local Docker Desktop image). Pods are labelled `app.kubernetes.io/managed-by=duckhouse-control-plane`.
- **AKS**: uses `ArmClient` (Azure SDK) to create AKS agent pools (`Count=1`, `Mode=User`) and `IKubernetes` to deploy one `duckhouse-runtime` pod pinned to that pool via `nodeSelector: kubernetes.azure.com/agentpool: <name>`. The pod runs the image's default entrypoint with no resource caps — it is the sole workload on the node. Pods are cleaned up before the agent pool is deleted.

`IKubernetes` is registered as a singleton for both backends (reads `~/.kube/config`).

## Key files

```
Nodes/
  INodeService.cs          — interface (List/Create/Remove/Stop/Start)
  Local/LocalNodeService.cs
  Aks/AksNodeService.cs
  Aks/AksNodeOptions.cs    — Section "NodeService:Aks"; includes RuntimeImage (ACR image ref)
Program.cs                 — backend selection and DI wiring
```

## Infrastructure (src/infra)

Bicep deploys the AKS cluster and ACR together (`main.bicep` → `modules/aks.bicep`):
- AKS uses a user-assigned managed identity; all node pools share the same kubelet identity.
- The kubelet identity has `AcrPull` on the ACR — no image pull secrets needed in pod specs.
- `acrLoginServer` is a deployment output; use it to set `NodeService:Aks:RuntimeImage` (e.g. `<acrLoginServer>/duckhouse-runtime:latest`).
