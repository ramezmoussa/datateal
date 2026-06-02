using Datateal.Auth.ApiKey;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Datateal.Auth;

/// <summary>
/// Extension methods for registering Datateal authentication services.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Configures OIDC authentication for the UI server using the provider specified in configuration.
    /// Reads "Authentication:Provider" to select the <see cref="IIdentityProviderSetup"/> implementation.
    /// The caller must register the appropriate <see cref="IIdentityProviderSetup"/> before calling this method.
    /// </summary>
    public static IServiceCollection AddDatatealWebAppAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        // The IIdentityProviderSetup implementation must already be registered
        // (e.g., by AddEntraIdAuthentication). Build a temporary SP to invoke it.
        using var sp = services.BuildServiceProvider();
        var setup = sp.GetRequiredService<IIdentityProviderSetup>();
        setup.ConfigureWebAppAuthentication(services, configuration);

        return services;
    }

    /// <summary>
    /// Configures API key authentication for backend services (orchestrator, control plane).
    /// Reads "ServiceAuth:ExpectedApiKey" from configuration.
    /// </summary>
    public static IServiceCollection AddDatatealApiKeyAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(ApiKeyAuthenticationOptions.Scheme)
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationOptions.Scheme, options =>
                {
                    options.ExpectedApiKey = configuration["ServiceAuth:ExpectedApiKey"]
                        ?? throw new InvalidOperationException(
                            "ServiceAuth:ExpectedApiKey is not configured.");
                });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Registers authorization policies using Datateal role constants.
    /// Used by the UI server to enforce role-based access on controllers and pages.
    /// </summary>
    public static IServiceCollection AddDatatealAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthPolicy.Admin, p =>
                p.RequireRole(DatatealRole.Admin))
            .AddPolicy(AuthPolicy.NodePoolManage, p =>
                p.RequireRole(DatatealRole.Admin, DatatealRole.NodePoolContributor))
            .AddPolicy(AuthPolicy.NodePoolOperate, p =>
                p.RequireRole(DatatealRole.Admin, DatatealRole.NodePoolContributor, DatatealRole.NodePoolOperator))
            .AddPolicy(AuthPolicy.JobManage, p =>
                p.RequireRole(DatatealRole.Admin, DatatealRole.JobContributor))
            .AddPolicy(AuthPolicy.JobOperate, p =>
                p.RequireRole(DatatealRole.Admin, DatatealRole.JobContributor, DatatealRole.JobOperator))
            .AddPolicy(AuthPolicy.JobRead, p =>
                p.RequireRole(DatatealRole.Admin, DatatealRole.JobContributor, DatatealRole.JobOperator, DatatealRole.JobReader))
            .AddPolicy(AuthPolicy.WorkspaceManage, p =>
                p.RequireRole(DatatealRole.Admin, DatatealRole.WorkspaceContributor))
            .AddPolicy(AuthPolicy.WorkspaceRead, p =>
                p.RequireRole(DatatealRole.Admin, DatatealRole.WorkspaceContributor, DatatealRole.WorkspaceReader))
            .AddPolicy(AuthPolicy.CatalogManage, p =>
                p.RequireRole(DatatealRole.Admin, DatatealRole.CatalogContributor))
            .AddPolicy(AuthPolicy.EnvironmentManage, p =>
                p.RequireRole(DatatealRole.Admin, DatatealRole.EnvironmentManager));

        return services;
    }
}
