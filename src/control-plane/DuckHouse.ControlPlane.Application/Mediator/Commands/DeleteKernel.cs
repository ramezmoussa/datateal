using DuckHouse.Core.Mediator;
using DuckHouse.ControlPlane.Core.Services;

namespace DuckHouse.ControlPlane.Application.Mediator.Commands;

public record DeleteKernelRequest(string NodeName, string KernelId) : IRequest;

internal class DeleteKernelHandler(INodeRuntimeClient runtimeClient) : IRequestHandler<DeleteKernelRequest>
{
    public Task Handle(DeleteKernelRequest request, CancellationToken cancellationToken) =>
        runtimeClient.DeleteKernelAsync(request.NodeName, request.KernelId, cancellationToken);
}
