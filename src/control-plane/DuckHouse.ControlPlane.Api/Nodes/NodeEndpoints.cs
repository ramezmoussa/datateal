namespace DuckHouse.ControlPlane.Api.Nodes;

public static class NodeEndpoints
{
    public static IEndpointRouteBuilder MapNodeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/nodes").WithTags("Nodes");

        group.MapGet("/", async (INodeService nodeService, CancellationToken ct) =>
            Results.Ok(await nodeService.ListNodesAsync(ct)))
            .WithName("ListNodes");

        group.MapPost("/", async (CreateNodeRequest request, INodeService nodeService, CancellationToken ct) =>
        {
            var node = await nodeService.CreateNodeAsync(request, ct);
            return Results.Created($"/nodes/{node.Name}", node);
        })
        .WithName("CreateNode");

        group.MapDelete("/{name}", async (string name, INodeService nodeService, CancellationToken ct) =>
        {
            await nodeService.RemoveNodeAsync(name, ct);
            return Results.NoContent();
        })
        .WithName("RemoveNode");

        group.MapPost("/{name}/stop", async (string name, INodeService nodeService, CancellationToken ct) =>
        {
            await nodeService.StopNodeAsync(name, ct);
            return Results.NoContent();
        })
        .WithName("StopNode");

        group.MapPost("/{name}/start", async (string name, INodeService nodeService, CancellationToken ct) =>
        {
            await nodeService.StartNodeAsync(name, ct);
            return Results.NoContent();
        })
        .WithName("StartNode");

        return endpoints;
    }
}
