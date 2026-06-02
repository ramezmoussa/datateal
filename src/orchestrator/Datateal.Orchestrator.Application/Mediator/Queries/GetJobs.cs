using Datateal.Core.Mediator;
using Datateal.Orchestrator.Core.Entities;
using Datateal.Orchestrator.Core.Repositories;

namespace Datateal.Orchestrator.Application.Mediator.Queries;

public record GetJobsRequest : IRequest<IReadOnlyList<Job>>;

internal class GetJobsHandler(IJobRepository jobRepository)
    : IRequestHandler<GetJobsRequest, IReadOnlyList<Job>>
{
    public async Task<IReadOnlyList<Job>> Handle(GetJobsRequest request, CancellationToken cancellationToken)
    {
        return await jobRepository.GetJobsAsync(cancellationToken);
    }
}
