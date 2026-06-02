using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Datateal.Ui.Server.Auth;

public static class LoginLogoutEndpoints
{
    public static IEndpointRouteBuilder MapLoginAndLogout(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("authentication");

        group.MapGet("/login", (string? returnUrl) => TypedResults.Challenge(
            new AuthenticationProperties { RedirectUri = returnUrl ?? "/" }))
            .AllowAnonymous();

        group.MapPost("/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        })
        .AllowAnonymous();

        return endpoints;
    }
}
