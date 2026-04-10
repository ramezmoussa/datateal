using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record DeleteNotebookRequest(Guid Id) : IRequest<bool>;

internal class DeleteNotebookHandler(IWorkspaceRepository repository) : IRequestHandler<DeleteNotebookRequest, bool>
{
    public Task<bool> Handle(DeleteNotebookRequest request, CancellationToken cancellationToken) =>
        repository.DeleteItemAsync(request.Id, cancellationToken);
}
