using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record StartNodeRequest(string Name) : IRequest;

internal class StartNodeHandler(INodeRepository nodeRepository) : IRequestHandler<StartNodeRequest>
{
    public Task Handle(StartNodeRequest request, CancellationToken cancellationToken) =>
        nodeRepository.StartNodeAsync(request.Name, cancellationToken);
}
