using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Workspace;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record UpdateNotebookRequest(Guid Id, string Title, string Content, Guid? FolderId) : IRequest<NotebookSummary?>;

internal class UpdateNotebookHandler(IWorkspaceRepository repository) : IRequestHandler<UpdateNotebookRequest, NotebookSummary?>
{
    public async Task<NotebookSummary?> Handle(UpdateNotebookRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.UpdateItemAsync(request.Id, request.Title, request.Content, request.FolderId, cancellationToken);
        return item is null ? null : new NotebookSummary(item.Id, item.Title, item.FolderId, item.CreatedAt, item.UpdatedAt);
    }
}
