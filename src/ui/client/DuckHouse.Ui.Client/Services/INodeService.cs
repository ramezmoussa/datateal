using DuckHouse.Core.Nodes;
using CreateNodeRequest = DuckHouse.Ui.Shared.Nodes.CreateNodeRequest;

namespace DuckHouse.Ui.Client.Services;

public interface INodeService
{
    Task<IReadOnlyList<NodeInfo>> GetNodesAsync(CancellationToken cancellationToken = default);
    Task<NodeInfo?> GetNodeAsync(string name, CancellationToken cancellationToken = default);
    Task<NodeInfo> CreateNodeAsync(CreateNodeRequest request, CancellationToken cancellationToken = default);
    Task RemoveNodeAsync(string name, CancellationToken cancellationToken = default);
    Task StopNodeAsync(string name, CancellationToken cancellationToken = default);
    Task StartNodeAsync(string name, CancellationToken cancellationToken = default);
}
