using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Datateal.Auth.Dev;

/// <summary>
/// Configures the dev (dummy) authentication scheme for the UI server.
/// Implements <see cref="ILoginLogoutEndpoints"/> so that login is a simple redirect
/// (no OIDC challenge) and logout is a no-op redirect to <c>/</c>.
/// </summary>
public class DevIdentityProviderSetup : IIdentityProviderSetup, ILoginLogoutEndpoints
{
    public void ConfigureWebAppAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var devSection = configuration.GetSection("Authentication:Dev");

        services.AddAuthentication(DevAuthenticationOptions.SchemeName)
            .AddScheme<DevAuthenticationOptions, DevAuthenticationHandler>(
                DevAuthenticationOptions.SchemeName,
                options =>
                {
                    devSection.GetSection("User").Bind(options.User);
                    var roles = devSection.GetSection("Roles").Get<List<string>>();
                    if (roles is not null)
                        options.Roles = roles;
                });

        services.AddCascadingAuthenticationState();
    }

    /// <summary>
    /// Login redirects straight to the return URL — the user is already authenticated.
    /// </summary>
    public void MapLoginEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/login", (string? returnUrl) =>
            TypedResults.Redirect(returnUrl ?? "/"))
            .AllowAnonymous();
    }

    /// <summary>
    /// Logout is a no-op redirect to <c>/</c>. There is no cookie or OIDC session to clear
    /// with the always-authenticated dev scheme.
    /// </summary>
    public void MapLogoutEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("/logout", () => TypedResults.Redirect("/"))
            .AllowAnonymous();
    }
}
