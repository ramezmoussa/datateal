using DuckHouse.Core.Nodes;
using DuckHouse.Data;
using DuckHouse.Orchestrator.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DuckHouse.Orchestrator.Infrastructure.Repositories;

internal class WheelPackageReader(IServiceScopeFactory scopeFactory) : IWheelPackageReader
{
    public async Task<IReadOnlyList<WheelContent>> GetWheelContentsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        if (idList.Count == 0) return [];

        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<DuckHouseDbContext>();
        return await db.WheelPackages
            .Where(p => idList.Contains(p.Id))
            .Select(p => new WheelContent(p.FileName, p.Data))
            .ToListAsync(ct);
    }
}

