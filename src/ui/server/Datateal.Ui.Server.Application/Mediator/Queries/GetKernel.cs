using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Queries;

public record GetKernelRequest(string NodeName, string KernelId) : IRequest<KernelInfo>;

internal class GetKernelHandler(IKernelRepository kernelRepository) : IRequestHandler<GetKernelRequest, KernelInfo>
{
    public Task<KernelInfo> Handle(GetKernelRequest request, CancellationToken cancellationToken) =>
        kernelRepository.GetKernelAsync(request.NodeName, request.KernelId, cancellationToken);
}
