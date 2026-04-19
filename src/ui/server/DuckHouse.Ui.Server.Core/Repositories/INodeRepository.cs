using DuckHouse.Core.Nodes;

namespace DuckHouse.Ui.Server.Core.Repositories;

public interface INodeRepository
{
    Task<IReadOnlyList<NodeInfo>> GetNodesAsync(CancellationToken cancellationToken = default);
    Task<NodeInfo?> GetNodeAsync(string name, CancellationToken cancellationToken = default);
    Task<NodeInfo> CreateNodeAsync(
        string name,
        string vmSize,
        TimeSpan? kernelIdleTimeout = null,
        TimeSpan? nodeIdleTimeout = null,
        string? kernelRequirements = null,
        IReadOnlyList<WheelContent>? wheelContents = null,
        IReadOnlyDictionary<string, string>? environmentVariables = null,
        IReadOnlyDictionary<string, string>? secrets = null,
        CancellationToken cancellationToken = default);

    /// <summary>Returns null if the control plane responds 409 (node already exists).</summary>
    Task<NodeInfo?> TryCreateNodeAsync(
        string name,
        string vmSize,
        TimeSpan? kernelIdleTimeout = null,
        TimeSpan? nodeIdleTimeout = null,
        string? kernelRequirements = null,
        IReadOnlyList<WheelContent>? wheelContents = null,
        IReadOnlyDictionary<string, string>? environmentVariables = null,
        IReadOnlyDictionary<string, string>? secrets = null,
        CancellationToken cancellationToken = default);
    Task RemoveNodeAsync(string name, CancellationToken cancellationToken = default);
}
