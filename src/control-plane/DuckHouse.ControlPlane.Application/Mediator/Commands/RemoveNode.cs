using DuckHouse.Core.Mediator;
using DuckHouse.ControlPlane.Core.Services;

namespace DuckHouse.ControlPlane.Application.Mediator.Commands;

public record RemoveNodeRequest(string Name) : IRequest;

internal class RemoveNodeHandler(INodeService nodeService) : IRequestHandler<RemoveNodeRequest>
{
    public Task Handle(RemoveNodeRequest request, CancellationToken cancellationToken) =>
        nodeService.RemoveNodeAsync(request.Name, cancellationToken);
}
