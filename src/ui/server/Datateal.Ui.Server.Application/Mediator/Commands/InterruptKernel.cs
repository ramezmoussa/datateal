using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record InterruptKernelRequest(string NodeName, string KernelId) : IRequest;

internal class InterruptKernelHandler(IKernelRepository kernelRepository) : IRequestHandler<InterruptKernelRequest>
{
    public Task Handle(InterruptKernelRequest request, CancellationToken cancellationToken) =>
        kernelRepository.InterruptKernelAsync(request.NodeName, request.KernelId, cancellationToken);
}
