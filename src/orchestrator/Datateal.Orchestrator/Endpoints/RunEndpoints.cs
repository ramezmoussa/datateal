using Datateal.Core.Mediator;
using Datateal.Orchestrator.Application.Mediator.Commands;
using Datateal.Orchestrator.Application.Mediator.Queries;

namespace Datateal.Orchestrator.Endpoints;

public static class RunEndpoints
{
    public static IEndpointRouteBuilder MapRunEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/runs").WithTags("Runs");

        group.MapGet("/", async (string? jobName, string? status, DateTime? from, DateTime? to, int? limit, int? offset, IMediator mediator, CancellationToken ct) =>
            Results.Ok(await mediator.SendAsync(
                new GetAllRunsRequest(jobName, status, from, to, limit ?? 100, offset ?? 0), ct)))
            .WithName("GetAllRuns");

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var run = await mediator.SendAsync(new GetJobRunRequest(id), ct);
            return run is null ? Results.NotFound() : Results.Ok(run);
        })
        .WithName("GetJobRun");

        group.MapGet("/{id:guid}/tasks/{taskRunId:guid}", async (Guid id, Guid taskRunId, IMediator mediator, CancellationToken ct) =>
        {
            var taskRun = await mediator.SendAsync(new GetTaskRunRequest(taskRunId), ct);
            return taskRun is null ? Results.NotFound() : Results.Ok(taskRun);
        })
        .WithName("GetTaskRun");

        group.MapGet("/{id:guid}/tasks/{taskRunId:guid}/cells", async (Guid id, Guid taskRunId, IMediator mediator, CancellationToken ct) =>
            Results.Ok(await mediator.SendAsync(new GetCellOutputsRequest(taskRunId), ct)))
            .WithName("GetCellOutputs");

        group.MapPost("/{id:guid}/cancel", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            await mediator.SendAsync(new CancelRunRequest(id), ct);
            return Results.NoContent();
        })
        .WithName("CancelRun");

        return endpoints;
    }
}
