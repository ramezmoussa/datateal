using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.ControlPlane.Core.Services;

namespace Datateal.ControlPlane.Application.Mediator.Commands;

public record CreateKernelRequest(string NodeName) : IRequest<KernelInfo>;

internal class CreateKernelHandler(INodeRuntimeClient runtimeClient) : IRequestHandler<CreateKernelRequest, KernelInfo>
{
    public Task<KernelInfo> Handle(CreateKernelRequest request, CancellationToken cancellationToken) =>
        runtimeClient.CreateKernelAsync(request.NodeName, cancellationToken);
}
