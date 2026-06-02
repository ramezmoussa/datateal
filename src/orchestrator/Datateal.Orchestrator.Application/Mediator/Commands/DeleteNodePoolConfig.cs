using Datateal.Core.Mediator;
using Datateal.Orchestrator.Core.Repositories;

namespace Datateal.Orchestrator.Application.Mediator.Commands;

public record DeleteNodePoolConfigRequest(Guid Id) : IRequest;

internal class DeleteNodePoolConfigHandler(INodePoolConfigRepository repository)
    : IRequestHandler<DeleteNodePoolConfigRequest>
{
    public async Task Handle(DeleteNodePoolConfigRequest request, CancellationToken cancellationToken)
    {
        await repository.DeleteAsync(request.Id, cancellationToken);
    }
}
