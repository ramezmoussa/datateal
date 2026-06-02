using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.ControlPlane.Core.Services;

namespace Datateal.ControlPlane.Application.Mediator.Commands;

public record CompleteKernelRequest(string NodeName, string KernelId, CompleteRequest Request) : IRequest<CompleteResponse>;

internal class CompleteKernelHandler(INodeRuntimeClient runtimeClient) : IRequestHandler<CompleteKernelRequest, CompleteResponse>
{
    public Task<CompleteResponse> Handle(CompleteKernelRequest request, CancellationToken cancellationToken) =>
        runtimeClient.CompleteAsync(request.NodeName, request.KernelId, request.Request, cancellationToken);
}
