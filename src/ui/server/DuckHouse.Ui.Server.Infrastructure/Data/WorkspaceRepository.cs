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

    public async Task<IReadOnlyList<Notebook>> GetNotebooksInAsync(Guid? folderId, CancellationToken cancellationToken = default)
    {
        var query = folderId.HasValue
            ? db.Notebooks.Where(n => n.FolderId == folderId)
            : db.Notebooks.Where(n => n.FolderId == null);

        return await query.OrderBy(n => n.Title).ToListAsync(cancellationToken);
    }

    public Task<Folder?> GetFolderAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Folders.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public Task<Notebook?> GetNotebookAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Notebooks.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

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

    public async Task<Notebook> CreateNotebookAsync(string title, string content, Guid? folderId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var notebook = new Notebook
        {
            Id = Guid.CreateVersion7(),
            Title = title,
            Content = content,
            FolderId = folderId,
            CreatedAt = now,
            UpdatedAt = now,
        };
        db.Notebooks.Add(notebook);
        await db.SaveChangesAsync(cancellationToken);
        return notebook;
    }

    public async Task<Notebook?> UpdateNotebookAsync(Guid id, string title, string content, Guid? folderId, CancellationToken cancellationToken = default)
    {
        var notebook = await db.Notebooks.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        if (notebook is null) return null;

        notebook.Title = title;
        notebook.Content = content;
        notebook.FolderId = folderId;
        notebook.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return notebook;
    }

    public async Task DeleteNotebookAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notebook = await db.Notebooks.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        if (notebook is not null)
        {
            db.Notebooks.Remove(notebook);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
