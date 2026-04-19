using DuckHouse.Core.Orchestration;
using DuckHouse.Orchestrator.Core.Enums;
using DuckHouse.Orchestrator.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DuckHouse.Orchestrator.Application.Engine;

/// <summary>
/// Runs once on startup to resume any in-progress job runs that were interrupted
/// by a service restart. This makes the orchestrator durable and restart-resilient.
/// </summary>
public class RecoveryService(
    IServiceScopeFactory scopeFactory,
    ILogger<RecoveryService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Small delay to let DI and other services initialize
        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);

        try
        {
            using var scope = scopeFactory.CreateScope();
            var jobRunRepo = scope.ServiceProvider.GetRequiredService<IJobRunRepository>();
            var runDispatcher = scope.ServiceProvider.GetRequiredService<RunDispatcher>();

            var activeRuns = await jobRunRepo.GetActiveRunsAsync(stoppingToken);

            if (activeRuns.Count == 0)
            {
                logger.LogInformation("Recovery: no in-progress runs to resume");
                return;
            }

            logger.LogInformation("Recovery: resuming {Count} in-progress run(s)", activeRuns.Count);

            foreach (var run in activeRuns)
            {
                if (run.Status is JobRunStatus.Running or JobRunStatus.Pending)
                {
                    logger.LogInformation("Recovery: re-dispatching run {RunId} (job {JobId}, status {Status})",
                        run.Id, run.JobId, run.Status);
                    runDispatcher.DispatchRun(run.Id);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during recovery");
        }
    }
}
