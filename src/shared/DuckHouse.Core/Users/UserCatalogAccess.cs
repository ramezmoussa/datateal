using DuckHouse.Core.Catalogs;

namespace DuckHouse.Core.Users;

/// <summary>
/// Grants a user access to a specific catalog.
/// </summary>
public class UserCatalogAccess
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public Guid CatalogId { get; set; }
    public Catalog Catalog { get; set; } = null!;
}
