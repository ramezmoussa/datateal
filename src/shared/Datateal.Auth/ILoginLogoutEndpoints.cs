using Microsoft.AspNetCore.Routing;

namespace Datateal.Auth;

/// <summary>
/// Optional extension for <see cref="IIdentityProviderSetup"/> implementations that need
/// custom login/logout endpoint behavior. When an <see cref="IIdentityProviderSetup"/>
/// also implements this interface, <c>LoginLogoutEndpoints.MapLoginAndLogout</c> delegates
/// to these methods instead of using the default OIDC challenge/sign-out behavior.
/// </summary>
public interface ILoginLogoutEndpoints
{
    void MapLoginEndpoint(RouteGroupBuilder group);
    void MapLogoutEndpoint(RouteGroupBuilder group);
}
