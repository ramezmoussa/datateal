using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DuckHouse.Core.Nodes;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Infrastructure.Repositories;

internal class NodeRepository(HttpClient httpClient) : INodeRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<IReadOnlyList<NodeInfo>> GetNodesAsync(CancellationToken cancellationToken = default) =>
        await httpClient.GetFromJsonAsync<List<NodeInfo>>("/nodes", JsonOptions, cancellationToken) ?? [];

    public async Task<NodeInfo?> GetNodeAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/nodes/{name}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NodeInfo>(JsonOptions, cancellationToken);
    }

    public async Task<NodeInfo> CreateNodeAsync(
        string name,
        string vmSize,
        TimeSpan? kernelIdleTimeout = null,
        TimeSpan? nodeIdleTimeout = null,
        string? kernelRequirements = null,
        IReadOnlyList<WheelContent>? wheelContents = null,
        IReadOnlyDictionary<string, string>? environmentVariables = null,
        IReadOnlyDictionary<string, string>? secrets = null,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/nodes",
            new CreateNodeRequest(name, vmSize, kernelIdleTimeout, nodeIdleTimeout, kernelRequirements, wheelContents, environmentVariables, secrets),
            JsonOptions,
            cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<NodeInfo>(JsonOptions, cancellationToken))!;
    }

    public async Task<NodeInfo?> TryCreateNodeAsync(
        string name,
        string vmSize,
        TimeSpan? kernelIdleTimeout = null,
        TimeSpan? nodeIdleTimeout = null,
        string? kernelRequirements = null,
        IReadOnlyList<WheelContent>? wheelContents = null,
        IReadOnlyDictionary<string, string>? environmentVariables = null,
        IReadOnlyDictionary<string, string>? secrets = null,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/nodes",
            new CreateNodeRequest(name, vmSize, kernelIdleTimeout, nodeIdleTimeout, kernelRequirements, wheelContents, environmentVariables, secrets),
            JsonOptions,
            cancellationToken);
        if (response.StatusCode == HttpStatusCode.Conflict) return null;
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<NodeInfo>(JsonOptions, cancellationToken))!;
    }

    public async Task RemoveNodeAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"/nodes/{name}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
