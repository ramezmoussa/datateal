using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DuckHouse.Core.Nodes;
using DuckHouse.Ui.Shared.Nodes;

namespace DuckHouse.Ui.Client.Services;

internal class InteractivePoolService(HttpClient httpClient) : IInteractivePoolService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<IReadOnlyList<InteractivePoolDto>> GetInteractivePoolsAsync(
        CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<List<InteractivePoolDto>>(
            "api/interactive-pools", JsonOptions, cancellationToken) ?? [];

    public async Task<NodeInfo?> EnsureNodeAsync(string poolName, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync(
            $"api/interactive-pools/{Uri.EscapeDataString(poolName)}/ensure-node",
            content: null, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NodeInfo>(JsonOptions, cancellationToken);
    }
}
