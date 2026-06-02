namespace Datateal.Ui.Client.Services;

/// <summary>
/// Stores AI provider API keys in AES-GCM-encrypted localStorage via the Web Crypto API.
/// Keys are scoped to this browser origin and cleared with site data.
/// Never transmitted to the server outside of an active AI chat request.
/// </summary>
public interface IAiCredentialService
{
    /// <summary>Gets the stored API key for the given provider, or null if not set.</summary>
    Task<string?> GetApiKeyAsync(string provider, CancellationToken ct = default);

    /// <summary>Stores the API key encrypted in localStorage.</summary>
    Task SetApiKeyAsync(string provider, string apiKey, CancellationToken ct = default);

    /// <summary>Removes the stored API key and its encryption key material.</summary>
    Task RemoveApiKeyAsync(string provider, CancellationToken ct = default);

    /// <summary>Returns true if an API key is currently stored for the given provider.</summary>
    Task<bool> HasApiKeyAsync(string provider, CancellationToken ct = default);
}
