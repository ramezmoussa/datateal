using DuckHouse.Core.Mediator;
using DuckHouse.Orchestrator.Core.Entities;
using DuckHouse.Orchestrator.Core.Repositories;

namespace DuckHouse.Orchestrator.Application.Mediator.Queries;

public record GetAllRunsRequest(
    string? JobName,
    string? Status,
    DateTime? From,
    DateTime? To,
    int Limit = 100,
    int Offset = 0) : IRequest<IReadOnlyList<JobRun>>;

internal class GetAllRunsHandler(IJobRunRepository jobRunRepository)
    : IRequestHandler<GetAllRunsRequest, IReadOnlyList<JobRun>>
{
    public async Task<IReadOnlyList<JobRun>> Handle(GetAllRunsRequest request, CancellationToken cancellationToken)
    {
        return await jobRunRepository.GetAllRunsAsync(
            request.JobName, request.Status, request.From, request.To,
            request.Limit, request.Offset, cancellationToken);
    }
}
