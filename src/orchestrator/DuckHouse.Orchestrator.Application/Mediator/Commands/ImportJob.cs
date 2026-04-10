using DuckHouse.Core.Mediator;
using DuckHouse.Orchestrator.Application.Yaml;
using DuckHouse.Orchestrator.Core.Entities;
using DuckHouse.Orchestrator.Core.Repositories;

namespace DuckHouse.Orchestrator.Application.Mediator.Commands;

public record ImportJobRequest(string Yaml) : IRequest<Job>;

internal class ImportJobHandler(
    YamlJobImporter importer,
    IJobRepository jobRepository) : IRequestHandler<ImportJobRequest, Job>
{
    public async Task<Job> Handle(ImportJobRequest request, CancellationToken cancellationToken)
    {
        var job = await importer.ImportAsync(request.Yaml, cancellationToken);
        return await jobRepository.CreateJobAsync(job, cancellationToken);
    }
}
