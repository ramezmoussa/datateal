using Datateal.Core.Mediator;
using Datateal.Orchestrator.Application.Yaml;
using Datateal.Orchestrator.Core.Repositories;

namespace Datateal.Orchestrator.Application.Mediator.Queries;

public record ExportJobRequest(Guid Id) : IRequest<string?>;

internal class ExportJobHandler(
    IJobRepository jobRepository,
    YamlJobSerializer serializer) : IRequestHandler<ExportJobRequest, string?>
{
    public async Task<string?> Handle(ExportJobRequest request, CancellationToken cancellationToken)
    {
        var job = await jobRepository.GetJobAsync(request.Id, cancellationToken);
        if (job is null) return null;

        return await serializer.SerializeAsync(job, cancellationToken);
    }
}
