using Datateal.Core.Catalogs;
using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Application.Mediator.Queries;
using Datateal.Ui.Server.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record ConnectKernelCatalogCommand(string NodeName, string KernelId, string CatalogName)
    : IRequest<ExecutionHandle>;

internal class ConnectKernelCatalogHandler(
    IMediator mediator,
    IKernelRepository kernelRepository,
    ILogger<ConnectKernelCatalogHandler> logger)
    : IRequestHandler<ConnectKernelCatalogCommand, ExecutionHandle>
{
    public async Task<ExecutionHandle> Handle(ConnectKernelCatalogCommand request, CancellationToken cancellationToken)
    {
        var resolved = await mediator.SendAsync(new ResolveCatalogsRequest([request.CatalogName]), cancellationToken);
        if (resolved.Count == 0)
            throw new InvalidOperationException($"Catalog '{request.CatalogName}' not found.");

        logger.LogInformation(
            "Security: Injecting credentials for catalog {CatalogName} into kernel {KernelId} on node {NodeName}",
            request.CatalogName, request.KernelId, request.NodeName);

        var script = CatalogSetupGenerator.GenerateAttachScript(resolved[0]);
        return await kernelRepository.StartExecuteAsync(request.NodeName, request.KernelId, new ExecuteRequest(script), cancellationToken);
    }
}
