using DuckHouse.Core.Kernels;
using DuckHouse.Ui.Application.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Queries;

public record GetKernelsRequest(string NodeName) : IRequest<IReadOnlyList<KernelInfo>>;

internal class GetKernelsHandler(IKernelRepository kernelRepository) : IRequestHandler<GetKernelsRequest, IReadOnlyList<KernelInfo>>
{
    public Task<IReadOnlyList<KernelInfo>> Handle(GetKernelsRequest request, CancellationToken cancellationToken) =>
        kernelRepository.GetKernelsAsync(request.NodeName, cancellationToken);
}
