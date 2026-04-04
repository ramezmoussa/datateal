using DuckHouse.Ui.Application.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record RemoveNodeRequest(string Name) : IRequest;

internal class RemoveNodeHandler(INodeRepository nodeRepository) : IRequestHandler<RemoveNodeRequest>
{
    public Task Handle(RemoveNodeRequest request, CancellationToken cancellationToken) =>
        nodeRepository.RemoveNodeAsync(request.Name, cancellationToken);
}
