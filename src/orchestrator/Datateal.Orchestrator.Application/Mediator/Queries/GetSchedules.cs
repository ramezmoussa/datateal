using Datateal.Core.Mediator;
using Datateal.Orchestrator.Core.Entities;
using Datateal.Orchestrator.Core.Repositories;

namespace Datateal.Orchestrator.Application.Mediator.Queries;

public record GetSchedulesRequest(Guid JobId) : IRequest<IReadOnlyList<JobSchedule>>;

internal class GetSchedulesHandler(IScheduleRepository scheduleRepository)
    : IRequestHandler<GetSchedulesRequest, IReadOnlyList<JobSchedule>>
{
    public async Task<IReadOnlyList<JobSchedule>> Handle(GetSchedulesRequest request, CancellationToken cancellationToken)
    {
        return await scheduleRepository.GetSchedulesForJobAsync(request.JobId, cancellationToken);
    }
}
