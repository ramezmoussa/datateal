using Microsoft.Extensions.DependencyInjection;

namespace Datateal.Auth.Dev;

/// <summary>
/// Extension methods for registering the dev (dummy) authentication provider.
/// </summary>
public static class DevAuthExtensions
{
    /// <summary>
    /// Registers the dev identity provider setup.
    /// Call this before <see cref="AuthenticationExtensions.AddDatatealWebAppAuthentication"/>.
    /// <para>
    /// <b>For local development only.</b> This provider auto-authenticates every request
    /// as the user configured in <c>Authentication:Dev</c>. Do not use in production.
    /// </para>
    /// </summary>
    public static IServiceCollection AddDevAuthentication(this IServiceCollection services)
    {
        services.AddSingleton<IIdentityProviderSetup, DevIdentityProviderSetup>();
        return services;
    }
}
