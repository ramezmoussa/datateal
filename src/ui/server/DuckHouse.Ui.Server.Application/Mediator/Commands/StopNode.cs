using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record StopNodeRequest(string Name) : IRequest;

internal class StopNodeHandler(INodeRepository nodeRepository) : IRequestHandler<StopNodeRequest>
{
    public Task Handle(StopNodeRequest request, CancellationToken cancellationToken) =>
        nodeRepository.StopNodeAsync(request.Name, cancellationToken);
}
