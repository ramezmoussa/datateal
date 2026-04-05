using DuckHouse.Core.Kernels;
using DuckHouse.Core.Mediator;
using DuckHouse.ControlPlane.Core.Services;

namespace DuckHouse.ControlPlane.Application.Mediator.Commands;

public record RestartKernelRequest(string NodeName, string KernelId) : IRequest<KernelInfo>;

internal class RestartKernelHandler(INodeRuntimeClient runtimeClient) : IRequestHandler<RestartKernelRequest, KernelInfo>
{
    public Task<KernelInfo> Handle(RestartKernelRequest request, CancellationToken cancellationToken) =>
        runtimeClient.RestartKernelAsync(request.NodeName, request.KernelId, cancellationToken);
}
