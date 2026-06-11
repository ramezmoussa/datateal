using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Datateal.Auth.Dev;

/// <summary>
/// Authentication handler for the dev (dummy) auth scheme.
/// Every request is automatically authenticated as the configured dummy user with the
/// configured roles. No OIDC redirect or credential prompt ever occurs.
/// </summary>
public class DevAuthenticationHandler(
    IOptionsMonitor<DevAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<DevAuthenticationOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new("preferred_username", Options.User.Email),
            new(ClaimTypes.Email, Options.User.Email),
            new(ClaimTypes.Name, Options.User.DisplayName),
        };

        if (Options.Roles is not null)
        {
            // Explicit roles configured — add them and mark as override so that
            // AppClaimsTransformation skips its admin seed list and DB role lookup.
            foreach (var role in Options.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            claims.Add(new Claim(DevAuthenticationOptions.RolesOverrideClaim, "true"));
        }
        // else: Roles is null → AppClaimsTransformation will look up roles from the DB
        //       by preferred_username (email), the same way a real OIDC login would.

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    /// <summary>
    /// Handles challenge by redirecting to the requested return URL.
    /// In practice this never fires because every request is already authenticated.
    /// </summary>
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Redirect(properties.RedirectUri ?? "/");
        return Task.CompletedTask;
    }
}
