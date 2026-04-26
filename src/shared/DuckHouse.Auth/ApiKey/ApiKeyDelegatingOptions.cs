namespace DuckHouse.Auth.ApiKey;

/// <summary>
/// Options for the outgoing API key delegating handler.
/// </summary>
public class ApiKeyDelegatingOptions
{
    public string ApiKey { get; set; } = string.Empty;
}
