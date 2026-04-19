using DuckHouse.Core.Nodes;
using DuckHouse.Ui.Shared.Nodes;

namespace DuckHouse.Ui.Client.Services;

public interface IInteractivePoolService
{
    Task<IReadOnlyList<InteractivePoolDto>> GetInteractivePoolsAsync(CancellationToken cancellationToken = default);
    Task<NodeInfo?> EnsureNodeAsync(string poolName, CancellationToken cancellationToken = default);
}
