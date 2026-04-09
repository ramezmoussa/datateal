using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Workspace;

namespace DuckHouse.Ui.Server.Application.Mediator.Queries;

public record GetWorkspaceRequest(Guid? FolderId = null) : IRequest<WorkspaceListing>;

internal class GetWorkspaceHandler(IWorkspaceRepository repository) : IRequestHandler<GetWorkspaceRequest, WorkspaceListing>
{
    public async Task<WorkspaceListing> Handle(GetWorkspaceRequest request, CancellationToken cancellationToken)
    {
        var folders = await repository.GetFoldersInAsync(request.FolderId, cancellationToken);
        var notebooks = await repository.GetNotebooksInAsync(request.FolderId, cancellationToken);

        var folderSummaries = folders
            .Select(f => new FolderSummary(f.Id, f.Name, f.ParentId, f.CreatedAt))
            .ToList();

        var notebookSummaries = notebooks
            .Select(n => new NotebookSummary(n.Id, n.Title, n.FolderId, n.CreatedAt, n.UpdatedAt))
            .ToList();

        return new WorkspaceListing(folderSummaries, notebookSummaries);
    }
}
