using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Workspace;

namespace DuckHouse.Ui.Server.Application.Mediator.Queries;

public record GetNotebookRequest(Guid Id) : IRequest<NotebookDetail?>;

internal class GetNotebookHandler(IWorkspaceRepository repository) : IRequestHandler<GetNotebookRequest, NotebookDetail?>
{
    public async Task<NotebookDetail?> Handle(GetNotebookRequest request, CancellationToken cancellationToken)
    {
        var item = await repository.GetItemAsync(request.Id, cancellationToken);
        return item is null
            ? null
            : new NotebookDetail(item.Id, item.Title, item.FolderId, item.CreatedAt, item.UpdatedAt, item.Content);
    }
}
