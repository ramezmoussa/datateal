using Microsoft.Extensions.Options;

namespace Datateal.Auth.ApiKey;

/// <summary>
/// Delegating handler that adds the X-Api-Key header to outgoing HTTP requests.
/// Registered on HttpClients that call downstream services (orchestrator, control plane).
/// </summary>
public class ApiKeyDelegatingHandler(IOptions<ApiKeyDelegatingOptions> options) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Remove(ApiKeyAuthenticationOptions.HeaderName);
        request.Headers.Add(ApiKeyAuthenticationOptions.HeaderName, options.Value.ApiKey);
        return base.SendAsync(request, cancellationToken);
    }
}
