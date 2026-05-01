using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DuckHouse.Auth;

/// <summary>
/// Configures OIDC authentication for the UI server.
/// Implement this interface per identity provider (Entra ID, Keycloak, etc.).
/// </summary>
public interface IIdentityProviderSetup
{
    /// <summary>
    /// Registers OIDC authentication services (OpenID Connect, cookie auth, token caches, etc.).
    /// </summary>
    void ConfigureWebAppAuthentication(IServiceCollection services, IConfiguration configuration);
}
