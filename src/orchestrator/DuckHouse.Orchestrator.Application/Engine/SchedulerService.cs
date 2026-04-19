using Cronos;
using DuckHouse.Core.Mediator;
using DuckHouse.Core.Orchestration;
using DuckHouse.Orchestrator.Application.Mediator.Commands;
using DuckHouse.Orchestrator.Core.Enums;
using DuckHouse.Orchestrator.Core.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DuckHouse.Orchestrator.Application.Engine;

/// <summary>
/// Background service that evaluates cron schedules and triggers job runs.
/// Reloads schedules from the database each cycle — no restart needed for schedule changes.
/// </summary>
public class SchedulerService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<SchedulerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalSeconds = int.TryParse(configuration["Scheduling:EvaluationIntervalSeconds"], out var parsed)
            ? parsed : 30;
        var interval = TimeSpan.FromSeconds(intervalSeconds);

        logger.LogInformation("Scheduler started with evaluation interval of {Interval}s",
            intervalSeconds);

        // Small initial delay to let the app fully start
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await EvaluateSchedulesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error evaluating schedules");
            }

            await Task.Delay(interval, stoppingToken);
        }

        logger.LogInformation("Scheduler stopped");
    }

    private async Task EvaluateSchedulesAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var scheduleRepo = scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
        var jobRepo = scope.ServiceProvider.GetRequiredService<IJobRepository>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var runDispatcher = scope.ServiceProvider.GetRequiredService<RunDispatcher>();

        var schedules = await scheduleRepo.GetEnabledSchedulesAsync(ct);
        var now = DateTimeOffset.UtcNow;

        foreach (var schedule in schedules)
        {
            try
            {
                // Parse cron expression (support both 5-field and 6-field)
                var format = schedule.CronExpression.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > 5
                    ? CronFormat.IncludeSeconds
                    : CronFormat.Standard;

                var cron = CronExpression.Parse(schedule.CronExpression, format);

                // Determine timezone
                var tz = !string.IsNullOrWhiteSpace(schedule.TimeZone)
                    ? TimeZoneInfo.FindSystemTimeZoneById(schedule.TimeZone)
                    : TimeZoneInfo.Utc;

                // Calculate next fire time from the last known fire time or from "now minus interval"
                var fromTime = schedule.NextFireTime.HasValue
                    ? new DateTimeOffset(schedule.NextFireTime.Value, TimeSpan.Zero)
                    : now.AddSeconds(-30);

                var nextFire = cron.GetNextOccurrence(fromTime, tz);

                if (nextFire.HasValue && nextFire.Value <= now)
                {
                    // Check that the job exists and is enabled
                    var job = await jobRepo.GetJobAsync(schedule.JobId, ct);
                    if (job is null || !job.IsEnabled)
                    {
                        logger.LogDebug("Skipping schedule {ScheduleId}: job {JobId} not found or disabled",
                            schedule.Id, schedule.JobId);
                        continue;
                    }

                    logger.LogInformation(
                        "Schedule {ScheduleId} fired for job '{JobName}' (cron: {Cron})",
                        schedule.Id, job.Name, schedule.CronExpression);

                    // Schedule-level parameter overrides
                    Dictionary<string, string>? parameters = schedule.Parameters;

                    // Trigger the job run
                    var run = await mediator.SendAsync(
                        new TriggerJobRequest(schedule.JobId, parameters, JobRunTrigger.Scheduled), ct);

                    // Dispatch it
                    runDispatcher.DispatchRun(run.Id);

                    // Update next fire time
                    var newNext = cron.GetNextOccurrence(now, tz);
                    schedule.NextFireTime = newNext?.UtcDateTime;
                    await scheduleRepo.UpdateScheduleAsync(schedule, ct);
                }
                else if (nextFire.HasValue)
                {
                    // Update next fire time in DB if changed
                    if (schedule.NextFireTime != nextFire.Value.UtcDateTime)
                    {
                        schedule.NextFireTime = nextFire.Value.UtcDateTime;
                        await scheduleRepo.UpdateScheduleAsync(schedule, ct);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error evaluating schedule {ScheduleId}", schedule.Id);
            }
        }
    }
}
