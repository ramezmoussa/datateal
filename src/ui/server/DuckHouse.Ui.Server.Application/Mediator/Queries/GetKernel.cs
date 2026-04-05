using DuckHouse.Core.Kernels;
using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Queries;

public record GetKernelRequest(string NodeName, string KernelId) : IRequest<KernelInfo>;

internal class GetKernelHandler(IKernelRepository kernelRepository) : IRequestHandler<GetKernelRequest, KernelInfo>
{
    public Task<KernelInfo> Handle(GetKernelRequest request, CancellationToken cancellationToken) =>
        kernelRepository.GetKernelAsync(request.NodeName, request.KernelId, cancellationToken);
}
