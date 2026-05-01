using System.Security.Claims;

namespace DuckHouse.Ui.Server.Core.Catalogs;

/// <summary>
/// Determines which catalogs the current user is allowed to access, based on their roles
/// and per-user catalog grants stored in the database.
/// </summary>
public interface ICatalogAccessService
{
    /// <summary>
    /// Returns <c>null</c> if the user has unrestricted access to all catalogs
    /// (Admin, CatalogContributor role, or <see cref="Core.Users.AppUser.HasAllCatalogAccess"/> flag).
    /// Otherwise returns the set of catalog IDs the user is explicitly allowed to access.
    /// </summary>
    Task<IReadOnlySet<Guid>?> GetAccessibleCatalogIdsAsync(ClaimsPrincipal user, CancellationToken ct = default);

    /// <summary>Returns <c>true</c> if the user can access the catalog with the given ID.</summary>
    Task<bool> HasAccessAsync(ClaimsPrincipal user, Guid catalogId, CancellationToken ct = default);

    /// <summary>Returns <c>true</c> if the user can access the catalog with the given name.</summary>
    Task<bool> HasAccessByNameAsync(ClaimsPrincipal user, string catalogName, CancellationToken ct = default);

    /// <summary>
    /// Filters <paramref name="catalogNames"/> to only those the user can access.
    /// Returns the original list unchanged if the user has unrestricted access.
    /// </summary>
    Task<IReadOnlyList<string>> FilterAccessibleNamesAsync(ClaimsPrincipal user, IReadOnlyList<string> catalogNames, CancellationToken ct = default);
}
