using DuckHouse.Data;
using DuckHouse.Orchestrator.Core.Entities;
using DuckHouse.Ui.Server.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DuckHouse.Ui.Server.Infrastructure.Repositories;

internal class InteractivePoolRepository(DuckHouseDbContext dbContext) : IInteractivePoolRepository
{
    public async Task<InteractivePoolInfo?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var pool = await dbContext.NodePoolConfigs
            .OfType<InteractiveNodePoolConfig>()
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);

        return pool is null ? null : ToInfo(pool);
    }

    public async Task<IReadOnlyList<InteractivePoolInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var pools = await dbContext.NodePoolConfigs
            .OfType<InteractiveNodePoolConfig>()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return pools.Select(ToInfo).ToList();
    }

    private static InteractivePoolInfo ToInfo(InteractiveNodePoolConfig pool) => new(
        pool.Id,
        pool.Name,
        pool.GetNodeName(),
        pool.VmSize,
        pool.KernelIdleTimeout,
        pool.NodeIdleTimeout,
        pool.KernelRequirements,
        pool.Description,
        pool.WheelPackageIds,
        pool.EnvironmentVariableIds,
        pool.SecretIds);
}
