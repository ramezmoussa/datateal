using DuckHouse.Core.Mediator;
using DuckHouse.Core.Nodes;
using DuckHouse.ControlPlane.Core.Services;

namespace DuckHouse.ControlPlane.Application.Mediator.Commands;

public record CreateNodeRequest(string Name, string? VmSize) : IRequest<NodeInfo>;

internal class CreateNodeHandler(INodeService nodeService) : IRequestHandler<CreateNodeRequest, NodeInfo>
{
    public Task<NodeInfo> Handle(CreateNodeRequest request, CancellationToken cancellationToken) =>
        nodeService.CreateNodeAsync(new DuckHouse.Core.Nodes.CreateNodeRequest(request.Name, request.VmSize), cancellationToken);
}
