using DuckHouse.Core.Users;
using DuckHouse.Data;
using DuckHouse.Ui.Server.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DuckHouse.Ui.Server.Infrastructure.Data;

internal class UserRepository(DuckHouseDbContext db) : IUserRepository
{
    public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct = default) =>
        await db.AppUsers
            .Include(u => u.CatalogAccessList)
                .ThenInclude(a => a.Catalog)
            .OrderBy(u => u.Email)
            .ToListAsync(ct);

    public Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.AppUsers
            .Include(u => u.CatalogAccessList)
                .ThenInclude(a => a.Catalog)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        db.AppUsers
            .Include(u => u.CatalogAccessList)
                .ThenInclude(a => a.Catalog)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = db.AppUsers.Where(u => u.Email == email);
        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);
        return query.AnyAsync(ct);
    }

    public async Task<AppUser> CreateAsync(AppUser user, CancellationToken ct = default)
    {
        db.AppUsers.Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }

    public async Task<AppUser?> UpdateAsync(AppUser user, CancellationToken ct = default)
    {
        var existing = await db.AppUsers
            .Include(u => u.CatalogAccessList)
            .FirstOrDefaultAsync(u => u.Id == user.Id, ct);
        if (existing is null) return null;

        existing.DisplayName = user.DisplayName;
        existing.IsEnabled = user.IsEnabled;
        existing.Roles = user.Roles;
        existing.HasAllCatalogAccess = user.HasAllCatalogAccess;
        existing.UpdatedAt = DateTime.UtcNow;

        // Sync catalog access list
        existing.CatalogAccessList.Clear();
        foreach (var access in user.CatalogAccessList)
        {
            existing.CatalogAccessList.Add(access);
        }

        await db.SaveChangesAsync(ct);

        // Re-load with Catalog navigation for DTO mapping
        await db.Entry(existing).Collection(u => u.CatalogAccessList).Query()
            .Include(a => a.Catalog).LoadAsync(ct);

        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await db.AppUsers.FindAsync([id], ct);
        if (user is null) return false;
        db.AppUsers.Remove(user);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
