using Datateal.Core.Nodes;
using Datateal.Ui.Shared.Nodes;

namespace Datateal.Ui.Client.Services;

public interface IInteractivePoolService
{
    Task<IReadOnlyList<InteractivePoolDto>> GetInteractivePoolsAsync(CancellationToken cancellationToken = default);
    Task<NodeInfo?> EnsureNodeAsync(string poolName, CancellationToken cancellationToken = default);
}
