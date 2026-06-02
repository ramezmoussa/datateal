using Datateal.Core.Nodes;

namespace Datateal.Orchestrator.Core.Interfaces;

public interface IWheelPackageReader
{
    Task<IReadOnlyList<WheelContent>> GetWheelContentsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
}
