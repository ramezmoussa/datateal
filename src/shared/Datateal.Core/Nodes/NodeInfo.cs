namespace Datateal.Core.Nodes;

public record NodeInfo(
    string Name,
    string ProvisioningState,
    string? VmSize,
    string? PowerState,
    NodeState State,
    TimeSpan? KernelIdleTimeout = null,
    TimeSpan? NodeIdleTimeout = null);
