using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record DeleteKernelRequest(string NodeName, string KernelId) : IRequest;

internal class DeleteKernelHandler(IKernelRepository kernelRepository) : IRequestHandler<DeleteKernelRequest>
{
    public Task Handle(DeleteKernelRequest request, CancellationToken cancellationToken) =>
        kernelRepository.DeleteKernelAsync(request.NodeName, request.KernelId, cancellationToken);
}
