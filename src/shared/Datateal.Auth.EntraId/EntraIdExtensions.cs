using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Datateal.Auth.EntraId;

/// <summary>
/// Extension methods for registering Entra ID authentication.
/// </summary>
public static class EntraIdExtensions
{
    /// <summary>
    /// Registers the Entra ID identity provider setup.
    /// Call this before <see cref="AuthenticationExtensions.AddDatatealWebAppAuthentication"/>.
    /// </summary>
    public static IServiceCollection AddEntraIdAuthentication(this IServiceCollection services)
    {
        services.AddSingleton<IIdentityProviderSetup, EntraIdIdentityProviderSetup>();
        return services;
    }
}
