using Microsoft.AspNetCore.Authentication;

namespace Datateal.Auth.Dev;

/// <summary>
/// Configures the identity for the automatically authenticated dummy user.
/// </summary>
public record DevUser
{
    /// <summary>The email address used as the user's identity claim (<c>preferred_username</c>).</summary>
    public string Email { get; init; } = "dev@local";

    /// <summary>The display name added as the <c>name</c> claim.</summary>
    public string DisplayName { get; init; } = "Local Dev User";
}

/// <summary>
/// Options for the dev (dummy) authentication scheme.
/// Populated from the <c>Authentication:Dev</c> configuration section.
/// </summary>
public class DevAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>The scheme name used to register this handler.</summary>
    public const string SchemeName = "DevAuth";

    /// <summary>
    /// Claim added to the identity when <see cref="Roles"/> is explicitly configured.
    /// <see cref="AppClaimsTransformation"/> checks this to skip the admin seed list
    /// and database role lookup, ensuring only the configured roles are applied.
    /// </summary>
    public const string RolesOverrideClaim = "datateal:dev-roles-override";

    /// <summary>Identity of the dummy user that every request is authenticated as.</summary>
    public DevUser User { get; set; } = new();

    /// <summary>
    /// Roles that are added directly as <see cref="System.Security.Claims.ClaimTypes.Role"/>
    /// claims on every authenticated request. Use <see cref="DatatealRole"/> constants.
    /// <para>
    /// When <c>null</c> (the default, and what you get when the <c>Roles</c> key is absent
    /// from <c>appsettings</c>), the user's roles and catalog permissions are instead fetched
    /// from the application database by <c>AppClaimsTransformation</c> using
    /// <see cref="DevUser.Email"/> as the lookup key — exactly as a real OIDC login would.
    /// This lets you impersonate any database user by setting their email and omitting
    /// <c>Roles</c>.
    /// </para>
    /// <para>
    /// When set to a list (even an empty one), those roles are used as-is and the database
    /// lookup is skipped entirely.
    /// </para>
    /// </summary>
    public List<string>? Roles { get; set; } = null;
}
