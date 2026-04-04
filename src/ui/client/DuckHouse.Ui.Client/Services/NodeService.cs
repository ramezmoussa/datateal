using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DuckHouse.Core.Nodes;
using CreateNodeRequest = DuckHouse.Ui.Shared.Nodes.CreateNodeRequest;

namespace DuckHouse.Ui.Client.Services;

internal class NodeService(HttpClient httpClient) : INodeService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<IReadOnlyList<NodeInfo>> GetNodesAsync(CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<List<NodeInfo>>("api/nodes", JsonOptions, cancellationToken) ?? [];

    public async Task<NodeInfo?> GetNodeAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"api/nodes/{name}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NodeInfo>(JsonOptions, cancellationToken);
    }

    public async Task<NodeInfo> CreateNodeAsync(CreateNodeRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/nodes", request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<NodeInfo>(JsonOptions, cancellationToken))!;
    }

    public async Task RemoveNodeAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"api/nodes/{name}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task StopNodeAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync($"api/nodes/{name}/stop", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task StartNodeAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync($"api/nodes/{name}/start", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
