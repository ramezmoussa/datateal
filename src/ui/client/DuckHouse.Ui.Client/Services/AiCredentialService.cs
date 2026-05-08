using Microsoft.JSInterop;

namespace DuckHouse.Ui.Client.Services;

/// <inheritdoc cref="IAiCredentialService"/>
public sealed class AiCredentialService(IJSRuntime js) : IAiCredentialService
{
    // Per-provider key IDs used as keys in dhEncryptedStorage
    private static string KeyId(string provider) => $"ai-key-{provider}";

    public async Task<string?> GetApiKeyAsync(string provider, CancellationToken ct = default)
    {
        try
        {
            return await js.InvokeAsync<string?>("dhEncryptedStorage.getItem", ct, KeyId(provider));
        }
        catch
        {
            return null;
        }
    }

    public async Task SetApiKeyAsync(string provider, string apiKey, CancellationToken ct = default)
    {
        await js.InvokeVoidAsync("dhEncryptedStorage.setItem", ct, KeyId(provider), apiKey);
    }

    public async Task RemoveApiKeyAsync(string provider, CancellationToken ct = default)
    {
        await js.InvokeVoidAsync("dhEncryptedStorage.removeItem", ct, KeyId(provider));
    }

    public async Task<bool> HasApiKeyAsync(string provider, CancellationToken ct = default)
    {
        try
        {
            return await js.InvokeAsync<bool>("dhEncryptedStorage.hasItem", ct, KeyId(provider));
        }
        catch
        {
            return false;
        }
    }
}
