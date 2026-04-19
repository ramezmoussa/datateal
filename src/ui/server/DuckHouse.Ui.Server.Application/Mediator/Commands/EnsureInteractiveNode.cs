using DuckHouse.Core.Mediator;
using DuckHouse.Core.Nodes;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Environment;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record EnsureInteractiveNodeRequest(string PoolName) : IRequest<NodeInfo?>;

internal class EnsureInteractiveNodeHandler(
    IInteractivePoolRepository poolRepository,
    INodeRepository nodeRepository,
    IWheelPackageRepository wheelPackageRepository,
    IMediator mediator)
    : IRequestHandler<EnsureInteractiveNodeRequest, NodeInfo?>
{
    public async Task<NodeInfo?> Handle(EnsureInteractiveNodeRequest request, CancellationToken cancellationToken)
    {
        var pool = await poolRepository.GetByNameAsync(request.PoolName, cancellationToken);
        if (pool is null) return null;

        var existing = await nodeRepository.GetNodeAsync(pool.NodeName, cancellationToken);
        if (existing is { State: NodeState.Running or NodeState.Creating })
            return existing;

        // Resolve dependencies before calling the control plane
        IReadOnlyList<WheelContent>? wheelContents = null;
        if (pool.WheelPackageIds is { Count: > 0 })
        {
            var packages = await wheelPackageRepository.GetByIdsAsync(pool.WheelPackageIds, cancellationToken);
            wheelContents = packages
                .Select(p => new WheelContent(p.FileName, p.Data))
                .ToList();
        }

        ResolvedEnvironment? resolved = null;
        if (pool.EnvironmentVariableIds is { Count: > 0 } || pool.SecretIds is { Count: > 0 })
        {
            resolved = await mediator.SendAsync(
                new Queries.ResolveEnvironmentRequest(pool.EnvironmentVariableIds, pool.SecretIds),
                cancellationToken);
        }

        var created = await nodeRepository.TryCreateNodeAsync(
            pool.NodeName,
            pool.VmSize,
            pool.KernelIdleTimeout,
            pool.NodeIdleTimeout,
            pool.KernelRequirements,
            wheelContents,
            resolved?.Variables,
            resolved?.Secrets,
            cancellationToken);

        if (created is not null)
            return created;

        // 409 Conflict — another request created the node concurrently; return what's there
        return await nodeRepository.GetNodeAsync(pool.NodeName, cancellationToken);
    }
}
