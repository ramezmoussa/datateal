using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Server.Core.Workspace;
using DuckHouse.Ui.Shared.Workspace;
using CoreItemType = DuckHouse.Ui.Server.Core.Workspace.WorkspaceItemType;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record CreateNotebookRequest(string Title, string Content, Guid? FolderId) : IRequest<NotebookSummary>;

internal class CreateNotebookHandler(IWorkspaceRepository repository) : IRequestHandler<CreateNotebookRequest, NotebookSummary>
{
    public async Task<NotebookSummary> Handle(CreateNotebookRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.CreateItemAsync(request.Title, request.Content, CoreItemType.Notebook, request.FolderId, cancellationToken);
        return new NotebookSummary(item.Id, item.Title, item.FolderId, item.CreatedAt, item.UpdatedAt);
    }
}
