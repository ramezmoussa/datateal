using DuckHouse.Core.Kernels;
using DuckHouse.Core.Mediator;
using DuckHouse.ControlPlane.Core.Services;

namespace DuckHouse.ControlPlane.Application.Mediator.Queries;

public record GetKernelsRequest(string NodeName) : IRequest<IReadOnlyList<KernelInfo>>;

internal class GetKernelsHandler(INodeRuntimeClient runtimeClient) : IRequestHandler<GetKernelsRequest, IReadOnlyList<KernelInfo>>
{
    public Task<IReadOnlyList<KernelInfo>> Handle(GetKernelsRequest request, CancellationToken cancellationToken) =>
        runtimeClient.ListKernelsAsync(request.NodeName, cancellationToken);
}
