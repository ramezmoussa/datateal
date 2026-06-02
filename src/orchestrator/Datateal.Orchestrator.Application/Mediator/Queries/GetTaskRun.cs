using Datateal.Core.Mediator;
using Datateal.Orchestrator.Core.Entities;
using Datateal.Orchestrator.Core.Repositories;

namespace Datateal.Orchestrator.Application.Mediator.Queries;

public record GetTaskRunRequest(Guid Id) : IRequest<TaskRun?>;

internal class GetTaskRunHandler(IJobRunRepository jobRunRepository)
    : IRequestHandler<GetTaskRunRequest, TaskRun?>
{
    public async Task<TaskRun?> Handle(GetTaskRunRequest request, CancellationToken cancellationToken)
    {
        return await jobRunRepository.GetTaskRunAsync(request.Id, cancellationToken);
    }
}
