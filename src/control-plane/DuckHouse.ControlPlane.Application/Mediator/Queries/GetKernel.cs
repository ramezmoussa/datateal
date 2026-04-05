using DuckHouse.Core.Kernels;
using DuckHouse.Core.Mediator;
using DuckHouse.ControlPlane.Core.Services;

namespace DuckHouse.ControlPlane.Application.Mediator.Queries;

public record GetKernelRequest(string NodeName, string KernelId) : IRequest<KernelInfo>;

internal class GetKernelHandler(INodeRuntimeClient runtimeClient) : IRequestHandler<GetKernelRequest, KernelInfo>
{
    public Task<KernelInfo> Handle(GetKernelRequest request, CancellationToken cancellationToken) =>
        runtimeClient.GetKernelAsync(request.NodeName, request.KernelId, cancellationToken);
}
