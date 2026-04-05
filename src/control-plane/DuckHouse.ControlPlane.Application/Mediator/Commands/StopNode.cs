using DuckHouse.Core.Mediator;
using DuckHouse.ControlPlane.Core.Services;

namespace DuckHouse.ControlPlane.Application.Mediator.Commands;

public record StopNodeRequest(string Name) : IRequest;

internal class StopNodeHandler(INodeService nodeService) : IRequestHandler<StopNodeRequest>
{
    public Task Handle(StopNodeRequest request, CancellationToken cancellationToken) =>
        nodeService.StopNodeAsync(request.Name, cancellationToken);
}
