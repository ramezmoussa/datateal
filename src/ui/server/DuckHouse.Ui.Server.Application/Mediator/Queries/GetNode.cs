using DuckHouse.Core.Nodes;
using DuckHouse.Ui.Application.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Queries;

public record GetNodeRequest(string Name) : IRequest<NodeInfo?>;

internal class GetNodeHandler(INodeRepository nodeRepository) : IRequestHandler<GetNodeRequest, NodeInfo?>
{
    public Task<NodeInfo?> Handle(GetNodeRequest request, CancellationToken cancellationToken) =>
        nodeRepository.GetNodeAsync(request.Name, cancellationToken);
}
