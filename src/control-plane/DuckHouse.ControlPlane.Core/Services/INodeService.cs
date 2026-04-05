using DuckHouse.Core.Nodes;

namespace DuckHouse.ControlPlane.Core.Services;

public interface INodeService
{
    Task<IReadOnlyList<NodeInfo>> ListNodesAsync(CancellationToken cancellationToken = default);
    Task<NodeInfo?> GetNodeAsync(string name, CancellationToken cancellationToken = default);
    Task<NodeInfo> CreateNodeAsync(CreateNodeRequest request, CancellationToken cancellationToken = default);
    Task RemoveNodeAsync(string name, CancellationToken cancellationToken = default);
    Task StopNodeAsync(string name, CancellationToken cancellationToken = default);
    Task StartNodeAsync(string name, CancellationToken cancellationToken = default);
}
