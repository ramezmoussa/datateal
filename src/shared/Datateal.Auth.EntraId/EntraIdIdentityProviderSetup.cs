using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace Datateal.Auth.EntraId;

/// <summary>
/// Configures Microsoft Entra ID (Azure AD) as the OIDC identity provider for the UI server.
/// </summary>
public class EntraIdIdentityProviderSetup : IIdentityProviderSetup
{
    public void ConfigureWebAppAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var entraSection = configuration.GetSection("Authentication:EntraId");

        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(entraSection);

        services.AddCascadingAuthenticationState();
    }
}
