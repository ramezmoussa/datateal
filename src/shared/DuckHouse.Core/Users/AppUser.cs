namespace DuckHouse.Core.Users;

/// <summary>
/// An application user. Users are managed in the app; authentication is delegated
/// to an external identity provider (e.g., Entra ID).
/// </summary>
public class AppUser
{
    public Guid Id { get; set; }

    /// <summary>
    /// Primary identifier for admin user management (email/UPN).
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// External identity provider object ID (e.g., Entra ID OID).
    /// Populated on first successful login for stable future lookups.
    /// </summary>
    public string? ExternalId { get; set; }

    public required string DisplayName { get; set; }

    public bool IsEnabled { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// If true, the user can access all catalogs (present and future).
    /// Admin and CatalogContributor users have implicit access regardless of this flag.
    /// </summary>
    public bool HasAllCatalogAccess { get; set; }

    /// <summary>
    /// Application roles. Stored as a JSON array (EF Core primitive collection).
    /// </summary>
    public List<string> Roles { get; set; } = [];

    /// <summary>
    /// Explicit catalog access grants (only relevant when <see cref="HasAllCatalogAccess"/> is false
    /// and the user is not Admin or CatalogContributor).
    /// </summary>
    public List<UserCatalogAccess> CatalogAccessList { get; set; } = [];
}
