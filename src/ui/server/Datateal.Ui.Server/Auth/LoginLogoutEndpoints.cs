using Datateal.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace Datateal.Ui.Server.Auth;

public static class LoginLogoutEndpoints
{
    public static IEndpointRouteBuilder MapLoginAndLogout(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("authentication");

        var provider = endpoints.ServiceProvider.GetRequiredService<IIdentityProviderSetup>();

        if (provider is ILoginLogoutEndpoints customEndpoints)
        {
            // Provider supplies its own login/logout behavior (e.g. dev dummy auth).
            customEndpoints.MapLoginEndpoint(group);
            customEndpoints.MapLogoutEndpoint(group);
        }
        else
        {
            // Default: OIDC challenge for login, cookie + OIDC sign-out for logout.
            group.MapGet("/login", (string? returnUrl) => TypedResults.Challenge(
                new AuthenticationProperties { RedirectUri = returnUrl ?? "/" }))
                .AllowAnonymous();

            group.MapPost("/logout", async (HttpContext context) =>
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            })
            .AllowAnonymous();
        }

        return endpoints;
    }
}
