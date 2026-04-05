using DuckHouse.Core.Mediator;
using DuckHouse.Core.Nodes;
using DuckHouse.ControlPlane.Core.Services;

namespace DuckHouse.ControlPlane.Application.Mediator.Queries;

public record GetNodeRequest(string Name) : IRequest<NodeInfo?>;

internal class GetNodeHandler(INodeService nodeService) : IRequestHandler<GetNodeRequest, NodeInfo?>
{
    public Task<NodeInfo?> Handle(GetNodeRequest request, CancellationToken cancellationToken) =>
        nodeService.GetNodeAsync(request.Name, cancellationToken);
}
