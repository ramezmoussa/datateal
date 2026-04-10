using DuckHouse.Ui.Server.Core.Workspace;

namespace DuckHouse.Ui.Server.Core.Repositories;

public interface IWorkspaceRepository
{
    Task<IReadOnlyList<Folder>> GetFoldersInAsync(Guid? parentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkspaceItem>> GetItemsInAsync(Guid? folderId, WorkspaceItemType? type = null, CancellationToken cancellationToken = default);
    Task<Folder?> GetFolderAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkspaceItem?> GetItemAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>Returns the ancestor chain from root down to the folder with <paramref name="id"/>, inclusive.</summary>
    Task<IReadOnlyList<Folder>> GetFolderAncestorsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Folder> CreateFolderAsync(string name, Guid? parentId, CancellationToken cancellationToken = default);
    Task<Folder?> UpdateFolderAsync(Guid id, string name, Guid? parentId, CancellationToken cancellationToken = default);
    Task DeleteFolderAsync(Guid id, CancellationToken cancellationToken = default);

    Task<WorkspaceItem> CreateItemAsync(string title, string content, WorkspaceItemType type, Guid? folderId, CancellationToken cancellationToken = default);
    Task<WorkspaceItem?> UpdateItemAsync(Guid id, string title, string content, Guid? folderId, CancellationToken cancellationToken = default);
    Task<bool> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default);
}
