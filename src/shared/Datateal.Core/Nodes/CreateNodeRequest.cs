namespace Datateal.Core.Nodes;

public record CreateNodeRequest(
    string Name,
    string? VmSize = null,
    TimeSpan? KernelIdleTimeout = null,
    TimeSpan? NodeIdleTimeout = null,
    string? KernelRequirements = null,
    IReadOnlyList<WheelContent>? WheelContents = null,
    IReadOnlyDictionary<string, string>? EnvironmentVariables = null,
    IReadOnlyDictionary<string, string>? Secrets = null);
