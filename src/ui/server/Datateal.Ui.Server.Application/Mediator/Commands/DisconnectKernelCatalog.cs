using Datateal.Core.Catalogs;
using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record DisconnectKernelCatalogCommand(string NodeName, string KernelId, string CatalogName)
    : IRequest<ExecutionHandle>;

internal class DisconnectKernelCatalogHandler(IKernelRepository kernelRepository)
    : IRequestHandler<DisconnectKernelCatalogCommand, ExecutionHandle>
{
    public Task<ExecutionHandle> Handle(DisconnectKernelCatalogCommand request, CancellationToken cancellationToken)
    {
        var script = CatalogSetupGenerator.GenerateDetachScript(request.CatalogName);
        return kernelRepository.StartExecuteAsync(request.NodeName, request.KernelId, new ExecuteRequest(script), cancellationToken);
    }
}
