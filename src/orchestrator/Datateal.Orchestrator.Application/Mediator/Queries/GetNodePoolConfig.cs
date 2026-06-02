using Datateal.Core.Mediator;
using Datateal.Orchestrator.Core.Entities;
using Datateal.Orchestrator.Core.Repositories;

namespace Datateal.Orchestrator.Application.Mediator.Queries;

public record GetNodePoolConfigRequest(Guid Id) : IRequest<NodePoolConfig?>;

internal class GetNodePoolConfigHandler(INodePoolConfigRepository repository)
    : IRequestHandler<GetNodePoolConfigRequest, NodePoolConfig?>
{
    public async Task<NodePoolConfig?> Handle(GetNodePoolConfigRequest request, CancellationToken cancellationToken)
    {
        return await repository.GetAsync(request.Id, cancellationToken);
    }
}
