using System.Security.Claims;
using DuckHouse.Auth;
using DuckHouse.Core.Catalogs;
using DuckHouse.Data;
using DuckHouse.Ui.Server.Core.Catalogs;
using Microsoft.EntityFrameworkCore;

namespace DuckHouse.Ui.Server.Infrastructure;

internal class CatalogAccessService(DuckHouseDbContext db) : ICatalogAccessService
{
    public async Task<IReadOnlySet<Guid>?> GetAccessibleCatalogIdsAsync(ClaimsPrincipal user, CancellationToken ct = default)
    {
        if (HasUnrestrictedRoles(user))
            return null;

        var appUser = await ResolveUserAsync(user, ct);
        if (appUser is null || appUser.HasAllCatalogAccess)
            return null;

        return appUser.CatalogAccessList.Select(a => a.CatalogId).ToHashSet();
    }

    public async Task<bool> HasAccessAsync(ClaimsPrincipal user, Guid catalogId, CancellationToken ct = default)
    {
        var accessibleIds = await GetAccessibleCatalogIdsAsync(user, ct);
        return accessibleIds is null || accessibleIds.Contains(catalogId);
    }

    public async Task<bool> HasAccessByNameAsync(ClaimsPrincipal user, string catalogName, CancellationToken ct = default)
    {
        if (HasUnrestrictedRoles(user))
            return true;

        var appUser = await ResolveUserAsync(user, ct);
        if (appUser is null || appUser.HasAllCatalogAccess)
            return true;

        var accessibleIds = appUser.CatalogAccessList.Select(a => a.CatalogId).ToHashSet();

        // Resolve catalog name → ID to check against the access list
        var catalog = await db.Catalogs
            .Where(c => c.Name == catalogName)
            .Select(c => new { c.Id })
            .FirstOrDefaultAsync(ct);

        return catalog is not null && accessibleIds.Contains(catalog.Id);
    }

    public async Task<IReadOnlyList<string>> FilterAccessibleNamesAsync(
        ClaimsPrincipal user, IReadOnlyList<string> catalogNames, CancellationToken ct = default)
    {
        if (catalogNames.Count == 0)
            return catalogNames;

        if (HasUnrestrictedRoles(user))
            return catalogNames;

        var appUser = await ResolveUserAsync(user, ct);
        if (appUser is null || appUser.HasAllCatalogAccess)
            return catalogNames;

        var accessibleIds = appUser.CatalogAccessList.Select(a => a.CatalogId).ToHashSet();

        var accessibleCatalogs = await db.Catalogs
            .Where(c => catalogNames.Contains(c.Name) && accessibleIds.Contains(c.Id))
            .Select(c => c.Name)
            .ToListAsync(ct);

        return accessibleCatalogs;
    }

    private static bool HasUnrestrictedRoles(ClaimsPrincipal user) =>
        user.IsInRole(DuckHouseRole.Admin) || user.IsInRole(DuckHouseRole.CatalogContributor);

    private async Task<UserWithCatalogAccess?> ResolveUserAsync(ClaimsPrincipal user, CancellationToken ct)
    {
        var externalId = user.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
            ?? user.FindFirstValue("oid");
        var email = user.FindFirstValue("preferred_username")
            ?? user.FindFirstValue(ClaimTypes.Email)
            ?? user.FindFirstValue("email");

        UserWithCatalogAccess? appUser = null;

        if (externalId is not null)
        {
            appUser = await db.AppUsers
                .Where(u => u.ExternalId == externalId)
                .Select(u => new UserWithCatalogAccess(
                    u.HasAllCatalogAccess,
                    u.CatalogAccessList.Select(a => new CatalogAccessEntry(a.CatalogId)).ToList()))
                .FirstOrDefaultAsync(ct);
        }

        if (appUser is null && email is not null)
        {
            appUser = await db.AppUsers
                .Where(u => u.Email == email)
                .Select(u => new UserWithCatalogAccess(
                    u.HasAllCatalogAccess,
                    u.CatalogAccessList.Select(a => new CatalogAccessEntry(a.CatalogId)).ToList()))
                .FirstOrDefaultAsync(ct);
        }

        return appUser;
    }

    private record UserWithCatalogAccess(bool HasAllCatalogAccess, List<CatalogAccessEntry> CatalogAccessList);
    private record CatalogAccessEntry(Guid CatalogId);
}
