using DuckHouse.Ui.Shared.Ai;

namespace DuckHouse.Ui.Client.Services;

/// <summary>
/// Stores the user's Azure OpenAI settings (deployment name and endpoint) in plain localStorage.
/// This data is non-sensitive.
/// </summary>
public interface IAiSettingsService
{
    Task<string> GetModelAsync(CancellationToken ct = default);
    Task SetModelAsync(string model, CancellationToken ct = default);

    Task<string> GetEndpointAsync(CancellationToken ct = default);
    Task SetEndpointAsync(string endpoint, CancellationToken ct = default);
}
