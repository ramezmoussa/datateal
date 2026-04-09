using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Workspace;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record CreateNotebookRequest(string Title, string Content, Guid? FolderId) : IRequest<NotebookSummary>;

internal class CreateNotebookHandler(IWorkspaceRepository repository) : IRequestHandler<CreateNotebookRequest, NotebookSummary>
{
    public async Task<NotebookSummary> Handle(CreateNotebookRequest request, CancellationToken cancellationToken)
    {
        var notebook = await repository.CreateNotebookAsync(request.Title, request.Content, request.FolderId, cancellationToken);
        return new NotebookSummary(notebook.Id, notebook.Title, notebook.FolderId, notebook.CreatedAt, notebook.UpdatedAt);
    }
}
