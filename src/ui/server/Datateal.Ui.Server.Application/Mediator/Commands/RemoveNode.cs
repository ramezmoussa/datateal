using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record RemoveNodeRequest(string Name) : IRequest;

internal class RemoveNodeHandler(INodeRepository nodeRepository) : IRequestHandler<RemoveNodeRequest>
{
    public Task Handle(RemoveNodeRequest request, CancellationToken cancellationToken) =>
        nodeRepository.RemoveNodeAsync(request.Name, cancellationToken);
}
