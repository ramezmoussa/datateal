using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Server.Core.Workspace;
using Microsoft.EntityFrameworkCore;

namespace DuckHouse.Ui.Server.Infrastructure.Data;

internal class WorkspaceRepository(UiDbContext db) : IWorkspaceRepository
{
    public async Task<IReadOnlyList<Folder>> GetFoldersInAsync(Guid? parentId, CancellationToken cancellationToken = default)
    {
        var query = parentId.HasValue
            ? db.Folders.Where(f => f.ParentId == parentId)
            : db.Folders.Where(f => f.ParentId == null);

        return await query.OrderBy(f => f.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WorkspaceItem>> GetItemsInAsync(Guid? folderId, WorkspaceItemType? type = null, CancellationToken cancellationToken = default)
    {
        var query = folderId.HasValue
            ? db.WorkspaceItems.Where(n => n.FolderId == folderId)
            : db.WorkspaceItems.Where(n => n.FolderId == null);

        if (type.HasValue)
            query = query.Where(n => n.ItemType == type.Value);

        return await query.OrderBy(n => n.Title).ToListAsync(cancellationToken);
    }

    public Task<Folder?> GetFolderAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Folders.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Folder>> GetFolderAncestorsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chain = new List<Folder>();
        var currentId = (Guid?)id;
        while (currentId.HasValue)
        {
            var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == currentId, cancellationToken);
            if (folder is null) break;
            chain.Insert(0, folder);
            currentId = folder.ParentId;
        }
        return chain;
    }

    public Task<WorkspaceItem?> GetItemAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.WorkspaceItems.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public async Task<Folder> CreateFolderAsync(string name, Guid? parentId, CancellationToken cancellationToken = default)
    {
        var folder = new Folder
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            ParentId = parentId,
            CreatedAt = DateTime.UtcNow,
        };
        db.Folders.Add(folder);
        await db.SaveChangesAsync(cancellationToken);
        return folder;
    }

    public async Task<Folder?> UpdateFolderAsync(Guid id, string name, Guid? parentId, CancellationToken cancellationToken = default)
    {
        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        if (folder is null) return null;

        folder.Name = name;
        folder.ParentId = parentId;
        await db.SaveChangesAsync(cancellationToken);
        return folder;
    }

    public async Task DeleteFolderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var folder = await db.Folders.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        if (folder is not null)
        {
            db.Folders.Remove(folder);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<WorkspaceItem> CreateItemAsync(string title, string content, WorkspaceItemType type, Guid? folderId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var item = new WorkspaceItem
        {
            Id = Guid.CreateVersion7(),
            ItemType = type,
            Title = title,
            Content = content,
            FolderId = folderId,
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.WorkspaceItems.Add(item);
        await db.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<WorkspaceItem?> UpdateItemAsync(Guid id, string title, string content, Guid? folderId, CancellationToken cancellationToken = default)
    {
        var item = await db.WorkspaceItems.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        if (item is null) return null;

        item.Title = title;
        item.Content = content;
        item.FolderId = folderId;
        item.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<bool> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await db.WorkspaceItems.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        if (item is null) return false;

        db.WorkspaceItems.Remove(item);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
