using DuckHouse.Core.Mediator;
using DuckHouse.Core.Nodes;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record CreateNodeRequest(string Name, string VmSize) : IRequest<NodeInfo>;

internal class CreateNodeHandler(INodeRepository nodeRepository) : IRequestHandler<CreateNodeRequest, NodeInfo>
{
    public Task<NodeInfo> Handle(CreateNodeRequest request, CancellationToken cancellationToken) =>
        nodeRepository.CreateNodeAsync(request.Name, request.VmSize, cancellationToken);
}