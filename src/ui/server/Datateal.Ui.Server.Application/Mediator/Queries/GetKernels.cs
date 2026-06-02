using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Queries;

public record GetKernelsRequest(string NodeName) : IRequest<IReadOnlyList<KernelInfo>>;

internal class GetKernelsHandler(IKernelRepository kernelRepository) : IRequestHandler<GetKernelsRequest, IReadOnlyList<KernelInfo>>
{
    public Task<IReadOnlyList<KernelInfo>> Handle(GetKernelsRequest request, CancellationToken cancellationToken) =>
        kernelRepository.GetKernelsAsync(request.NodeName, cancellationToken);
}
