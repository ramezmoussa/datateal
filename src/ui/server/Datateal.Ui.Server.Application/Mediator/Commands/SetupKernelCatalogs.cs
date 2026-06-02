using Datateal.Core.Catalogs;
using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Application.Mediator.Queries;
using Datateal.Ui.Server.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record SetupKernelCatalogsCommand(string NodeName, string KernelId, IReadOnlyList<string> CatalogNames)
    : IRequest<ExecutionHandle>;

internal class SetupKernelCatalogsHandler(
    IMediator mediator,
    IKernelRepository kernelRepository,
    ILogger<SetupKernelCatalogsHandler> logger)
    : IRequestHandler<SetupKernelCatalogsCommand, ExecutionHandle>
{
    public async Task<ExecutionHandle> Handle(SetupKernelCatalogsCommand request, CancellationToken cancellationToken)
    {
        var resolved = await mediator.SendAsync(new ResolveCatalogsRequest(request.CatalogNames), cancellationToken);

        logger.LogInformation(
            "Security: Injecting credentials for catalog(s) {CatalogNames} into kernel {KernelId} on node {NodeName}",
            string.Join(", ", resolved.Select(c => c.Name)), request.KernelId, request.NodeName);

        var script = CatalogSetupGenerator.GenerateSetupScript(resolved);
        return await kernelRepository.StartExecuteAsync(request.NodeName, request.KernelId, new ExecuteRequest(script), cancellationToken);
    }
}
