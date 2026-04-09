using DuckHouse.Ui.Server.Core.Workspace;

namespace DuckHouse.Ui.Server.Core.Repositories;

public interface IWorkspaceRepository
{
    Task<IReadOnlyList<Folder>> GetFoldersInAsync(Guid? parentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notebook>> GetNotebooksInAsync(Guid? folderId, CancellationToken cancellationToken = default);
    Task<Folder?> GetFolderAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Notebook?> GetNotebookAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Folder> CreateFolderAsync(string name, Guid? parentId, CancellationToken cancellationToken = default);
    Task<Folder?> UpdateFolderAsync(Guid id, string name, Guid? parentId, CancellationToken cancellationToken = default);
    Task DeleteFolderAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Notebook> CreateNotebookAsync(string title, string content, Guid? folderId, CancellationToken cancellationToken = default);
    Task<Notebook?> UpdateNotebookAsync(Guid id, string title, string content, Guid? folderId, CancellationToken cancellationToken = default);
    Task DeleteNotebookAsync(Guid id, CancellationToken cancellationToken = default);
}
