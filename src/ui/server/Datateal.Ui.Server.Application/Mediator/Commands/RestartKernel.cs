using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record RestartKernelRequest(string NodeName, string KernelId) : IRequest<KernelInfo>;

internal class RestartKernelHandler(IKernelRepository kernelRepository) : IRequestHandler<RestartKernelRequest, KernelInfo>
{
    public Task<KernelInfo> Handle(RestartKernelRequest request, CancellationToken cancellationToken) =>
        kernelRepository.RestartKernelAsync(request.NodeName, request.KernelId, cancellationToken);
}
