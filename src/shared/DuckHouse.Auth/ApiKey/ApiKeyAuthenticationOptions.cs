using Microsoft.AspNetCore.Authentication;

namespace DuckHouse.Auth.ApiKey;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string Scheme = "ApiKey";
    public const string HeaderName = "X-Api-Key";

    /// <summary>
    /// The expected API key that incoming requests must provide.
    /// </summary>
    public string ExpectedApiKey { get; set; } = string.Empty;
}
