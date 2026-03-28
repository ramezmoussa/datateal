namespace DuckHouse.ControlPlane.Api.Nodes;

public record NodeInfo(string Name, string ProvisioningState, string? VmSize = null, string? PowerState = null);
