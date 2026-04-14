using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DuckHouse.Ui.Shared.Environment;

namespace DuckHouse.Ui.Client.Services;

internal class EnvironmentService(HttpClient httpClient) : IEnvironmentService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    // ── Variables ────────────────────────────────────────────────────────

    public async Task<IReadOnlyList<EnvironmentVariableDto>> GetVariablesAsync(CancellationToken ct) =>
        await httpClient.GetFromJsonAsync<IReadOnlyList<EnvironmentVariableDto>>(
            "api/environment/variables", JsonOptions, ct) ?? [];

    public async Task<EnvironmentVariableDto> CreateVariableAsync(CreateEnvironmentVariableRequest request, CancellationToken ct)
    {
        var response = await httpClient.PostAsJsonAsync("api/environment/variables", request, JsonOptions, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<EnvironmentVariableDto>(JsonOptions, ct))!;
    }

    public async Task<EnvironmentVariableDto?> UpdateVariableAsync(Guid id, UpdateEnvironmentVariableRequest request, CancellationToken ct)
    {
        var response = await httpClient.PutAsJsonAsync($"api/environment/variables/{id}", request, JsonOptions, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EnvironmentVariableDto>(JsonOptions, ct);
    }

    public async Task DeleteVariableAsync(Guid id, CancellationToken ct)
    {
        var response = await httpClient.DeleteAsync($"api/environment/variables/{id}", ct);
        response.EnsureSuccessStatusCode();
    }

    // ── Secrets ──────────────────────────────────────────────────────────

    public async Task<IReadOnlyList<SecretDto>> GetSecretsAsync(CancellationToken ct) =>
        await httpClient.GetFromJsonAsync<IReadOnlyList<SecretDto>>(
            "api/environment/secrets", JsonOptions, ct) ?? [];

    public async Task<SecretDto> CreateSecretAsync(CreateSecretRequest request, CancellationToken ct)
    {
        var response = await httpClient.PostAsJsonAsync("api/environment/secrets", request, JsonOptions, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SecretDto>(JsonOptions, ct))!;
    }

    public async Task<SecretDto?> UpdateSecretAsync(Guid id, UpdateSecretRequest request, CancellationToken ct)
    {
        var response = await httpClient.PutAsJsonAsync($"api/environment/secrets/{id}", request, JsonOptions, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SecretDto>(JsonOptions, ct);
    }

    public async Task DeleteSecretAsync(Guid id, CancellationToken ct)
    {
        var response = await httpClient.DeleteAsync($"api/environment/secrets/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}
