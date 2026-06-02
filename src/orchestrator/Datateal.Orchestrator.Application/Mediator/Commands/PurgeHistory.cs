using Datateal.Core.Mediator;
using Datateal.Orchestrator.Core.Repositories;

namespace Datateal.Orchestrator.Application.Mediator.Commands;

public record PurgeHistoryRequest(int RetentionDays = 30) : IRequest<int>;

internal class PurgeHistoryHandler(IJobRunRepository jobRunRepository)
    : IRequestHandler<PurgeHistoryRequest, int>
{
    public async Task<int> Handle(PurgeHistoryRequest request, CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddDays(-request.RetentionDays);
        return await jobRunRepository.PurgeRunsOlderThanAsync(cutoff, cancellationToken);
    }
}
