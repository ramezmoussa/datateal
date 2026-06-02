using Datateal.Core.Mediator;
using Datateal.Orchestrator.Application.Engine;
using Datateal.Orchestrator.Core.Entities;
using Datateal.Orchestrator.Core.Repositories;

namespace Datateal.Orchestrator.Application.Mediator.Commands;

public record UpdateScheduleRequest(
    Guid Id,
    string CronExpression,
    bool IsEnabled,
    string? TimeZone,
    Dictionary<string, string>? Parameters) : IRequest<JobSchedule?>;

internal class UpdateScheduleHandler(
    IScheduleRepository scheduleRepository,
    SchedulesManager schedulesManager)
    : IRequestHandler<UpdateScheduleRequest, JobSchedule?>
{
    public async Task<JobSchedule?> Handle(UpdateScheduleRequest request, CancellationToken cancellationToken)
    {
        var schedule = new JobSchedule
        {
            Id = request.Id,
            CronExpression = request.CronExpression,
            IsEnabled = request.IsEnabled,
            TimeZone = request.TimeZone,
            Parameters = request.Parameters,
        };

        var updated = await scheduleRepository.UpdateScheduleAsync(schedule, cancellationToken);
        if (updated is not null)
        {
            await schedulesManager.UpdateScheduleAsync(updated, cancellationToken);
        }
        return updated;
    }
}
