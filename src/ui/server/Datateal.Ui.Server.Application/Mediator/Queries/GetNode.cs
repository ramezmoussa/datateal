using Datateal.Core.Mediator;
using Datateal.Core.Nodes;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Queries;

public record GetNodeRequest(string Name) : IRequest<NodeInfo?>;

internal class GetNodeHandler(INodeRepository nodeRepository) : IRequestHandler<GetNodeRequest, NodeInfo?>
{
    public Task<NodeInfo?> Handle(GetNodeRequest request, CancellationToken cancellationToken) =>
        nodeRepository.GetNodeAsync(request.Name, cancellationToken);
}
