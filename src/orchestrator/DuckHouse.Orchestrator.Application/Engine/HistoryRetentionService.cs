using DuckHouse.Orchestrator.Core.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DuckHouse.Orchestrator.Application.Engine;

/// <summary>
/// Periodically purges old job run history based on a configurable retention period.
/// Runs once per hour by default.
/// </summary>
public class HistoryRetentionService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<HistoryRetentionService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var retentionDays = int.TryParse(configuration["History:RetentionDays"], out var days)
            ? days : 30;
        var intervalHours = int.TryParse(configuration["History:PurgeIntervalHours"], out var hours)
            ? hours : 1;

        if (retentionDays <= 0)
        {
            logger.LogInformation("History retention is disabled (RetentionDays <= 0)");
            return;
        }

        logger.LogInformation(
            "History retention started: purging runs older than {Days} days every {Hours} hour(s)",
            retentionDays, intervalHours);

        // Initial delay
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var jobRunRepo = scope.ServiceProvider.GetRequiredService<IJobRunRepository>();

                var cutoff = DateTime.UtcNow.AddDays(-retentionDays);
                var purged = await jobRunRepo.PurgeRunsOlderThanAsync(cutoff, stoppingToken);

                if (purged > 0)
                    logger.LogInformation("Purged {Count} job run(s) older than {Cutoff:yyyy-MM-dd}", purged, cutoff);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error purging job run history");
            }

            await Task.Delay(TimeSpan.FromHours(intervalHours), stoppingToken);
        }
    }
}
