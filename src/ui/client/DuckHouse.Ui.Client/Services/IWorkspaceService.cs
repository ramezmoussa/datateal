using DuckHouse.Ui.Shared.Workspace;

namespace DuckHouse.Ui.Client.Services;

public interface IWorkspaceService
{
    Task<WorkspaceListing> GetRootAsync(CancellationToken cancellationToken = default);
    Task<WorkspaceListing> GetFolderAsync(Guid folderId, CancellationToken cancellationToken = default);
    Task<NotebookDetail?> GetNotebookAsync(Guid id, CancellationToken cancellationToken = default);

    Task<FolderSummary> CreateFolderAsync(CreateFolderRequest request, CancellationToken cancellationToken = default);
    Task<FolderSummary?> UpdateFolderAsync(Guid id, UpdateFolderRequest request, CancellationToken cancellationToken = default);
    Task DeleteFolderAsync(Guid id, CancellationToken cancellationToken = default);

    Task<NotebookSummary> CreateNotebookAsync(CreateNotebookRequest request, CancellationToken cancellationToken = default);
    Task<NotebookSummary?> UpdateNotebookAsync(Guid id, UpdateNotebookRequest request, CancellationToken cancellationToken = default);
    Task DeleteNotebookAsync(Guid id, CancellationToken cancellationToken = default);
}
