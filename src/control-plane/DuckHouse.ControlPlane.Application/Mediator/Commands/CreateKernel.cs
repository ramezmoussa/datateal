using DuckHouse.Core.Kernels;
using DuckHouse.Core.Mediator;
using DuckHouse.ControlPlane.Core.Services;

namespace DuckHouse.ControlPlane.Application.Mediator.Commands;

public record CreateKernelRequest(string NodeName) : IRequest<KernelInfo>;

internal class CreateKernelHandler(INodeRuntimeClient runtimeClient) : IRequestHandler<CreateKernelRequest, KernelInfo>
{
    public Task<KernelInfo> Handle(CreateKernelRequest request, CancellationToken cancellationToken) =>
        runtimeClient.CreateKernelAsync(request.NodeName, cancellationToken);
}
