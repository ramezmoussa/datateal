using DuckHouse.Core.Mediator;
using DuckHouse.Core.Nodes;
using DuckHouse.ControlPlane.Core.Services;

namespace DuckHouse.ControlPlane.Application.Mediator.Queries;

public record GetNodesRequest : IRequest<IReadOnlyList<NodeInfo>>;

internal class GetNodesHandler(INodeService nodeService) : IRequestHandler<GetNodesRequest, IReadOnlyList<NodeInfo>>
{
    public Task<IReadOnlyList<NodeInfo>> Handle(GetNodesRequest request, CancellationToken cancellationToken) =>
        nodeService.ListNodesAsync(cancellationToken);
}
