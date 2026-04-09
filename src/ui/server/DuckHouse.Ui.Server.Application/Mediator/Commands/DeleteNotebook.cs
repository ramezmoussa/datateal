using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record DeleteNotebookRequest(Guid Id) : IRequest<bool>;

internal class DeleteNotebookHandler(IWorkspaceRepository repository) : IRequestHandler<DeleteNotebookRequest, bool>
{
    public async Task<bool> Handle(DeleteNotebookRequest request, CancellationToken cancellationToken)
    {
        var notebook = await repository.GetNotebookAsync(request.Id, cancellationToken);
        if (notebook is null) return false;
        await repository.DeleteNotebookAsync(request.Id, cancellationToken);
        return true;
    }
}
