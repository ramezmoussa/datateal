namespace DuckHouse.Core.Nodes;

public record CreateNodeRequest(string Name, string? VmSize = null);
