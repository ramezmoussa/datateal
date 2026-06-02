using Microsoft.JSInterop;

namespace Datateal.Ui.Client.Services;

/// <inheritdoc cref="IAiSettingsService"/>
public sealed class AiSettingsService(IJSRuntime js) : IAiSettingsService
{
    private const string ModelKey = "dh-ai-model";
    private const string EndpointKey = "dh-ai-endpoint";

    public async Task<string> GetModelAsync(CancellationToken ct = default)
    {
        try
        {
            var stored = await js.InvokeAsync<string?>("localStorage.getItem", ct, ModelKey);
            if (!string.IsNullOrEmpty(stored))
                return stored;
        }
        catch { }
        return string.Empty;
    }

    public async Task SetModelAsync(string model, CancellationToken ct = default)
    {
        await js.InvokeVoidAsync("localStorage.setItem", ct, ModelKey, model);
    }

    public async Task<string> GetEndpointAsync(CancellationToken ct = default)
    {
        try
        {
            var stored = await js.InvokeAsync<string?>("localStorage.getItem", ct, EndpointKey);
            if (!string.IsNullOrEmpty(stored))
                return stored;
        }
        catch { }
        return string.Empty;
    }

    public async Task SetEndpointAsync(string endpoint, CancellationToken ct = default)
    {
        await js.InvokeVoidAsync("localStorage.setItem", ct, EndpointKey, endpoint);
    }
}
