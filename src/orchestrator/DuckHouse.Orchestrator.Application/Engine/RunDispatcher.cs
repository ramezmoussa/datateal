using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DuckHouse.Orchestrator.Application.Engine;

/// <summary>
/// Singleton that bridges trigger requests and the RunCoordinator.
/// When a job run is created (via API or scheduler), RunDispatcher picks it up
/// and starts coordinating it in the background.
/// </summary>
public class RunDispatcher(IServiceScopeFactory scopeFactory, ILogger<RunDispatcher> logger)
{
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _activeRuns = new();

    /// <summary>
    /// Dispatches a job run for background execution.
    /// </summary>
    public void DispatchRun(Guid jobRunId)
    {
        var cts = new CancellationTokenSource();
        if (!_activeRuns.TryAdd(jobRunId, cts))
        {
            logger.LogWarning("Run {RunId} is already being dispatched", jobRunId);
            cts.Dispose();
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var coordinator = scope.ServiceProvider.GetRequiredService<RunCoordinator>();
                await coordinator.ExecuteRunAsync(jobRunId, cts.Token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error executing run {RunId}", jobRunId);
            }
            finally
            {
                _activeRuns.TryRemove(jobRunId, out var removed);
                removed?.Dispose();
            }
        });

        logger.LogInformation("Dispatched run {RunId}", jobRunId);
    }

    /// <summary>
    /// Cancels an active run.
    /// </summary>
    public Task CancelRunAsync(Guid jobRunId)
    {
        if (_activeRuns.TryGetValue(jobRunId, out var cts))
        {
            logger.LogInformation("Cancelling run {RunId}", jobRunId);
            cts.Cancel();
        }
        else
        {
            logger.LogWarning("Run {RunId} not found in active runs", jobRunId);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns whether a run is currently active.
    /// </summary>
    public bool IsRunActive(Guid jobRunId) => _activeRuns.ContainsKey(jobRunId);
}
