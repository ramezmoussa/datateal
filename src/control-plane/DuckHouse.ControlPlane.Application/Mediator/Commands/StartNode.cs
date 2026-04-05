using DuckHouse.Core.Mediator;
using DuckHouse.ControlPlane.Core.Services;

namespace DuckHouse.ControlPlane.Application.Mediator.Commands;

public record StartNodeRequest(string Name) : IRequest;

internal class StartNodeHandler(INodeService nodeService) : IRequestHandler<StartNodeRequest>
{
    public Task Handle(StartNodeRequest request, CancellationToken cancellationToken) =>
        nodeService.StartNodeAsync(request.Name, cancellationToken);
}
