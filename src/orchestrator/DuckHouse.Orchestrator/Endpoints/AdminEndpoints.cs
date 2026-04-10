using DuckHouse.Core.Mediator;
using DuckHouse.Orchestrator.Application.Mediator.Commands;

namespace DuckHouse.Orchestrator.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/admin").WithTags("Admin");

        group.MapPost("/purge-history", async (PurgeHistoryBody? body, IMediator mediator, CancellationToken ct) =>
        {
            var retentionDays = body?.RetentionDays ?? 30;
            var purged = await mediator.SendAsync(new PurgeHistoryRequest(retentionDays), ct);
            return Results.Ok(new { purged, retentionDays });
        })
        .WithName("PurgeHistory");

        return endpoints;
    }
}

public record PurgeHistoryBody(int RetentionDays = 30);
