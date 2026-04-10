using System.Data.Common;
using DuckHouse.Orchestrator.Core.Interfaces;
using DuckHouse.Orchestrator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DuckHouse.Orchestrator.Infrastructure.Repositories;

internal class WorkspaceReader(OrchestratorDbContext dbContext) : IWorkspaceReader
{
    public async Task<WorkspaceItemContent?> GetNotebookContentAsync(Guid notebookId, CancellationToken ct)
    {
        return await ReadItemAsync(notebookId, "Notebook", ct);
    }

    public async Task<WorkspaceItemContent?> GetQueryContentAsync(Guid queryId, CancellationToken ct)
    {
        return await ReadItemAsync(queryId, "Query", ct);
    }

    public async Task<Guid?> ResolveNotebookIdByPathAsync(string path, CancellationToken ct)
    {
        return await ResolveItemIdByPathAsync(path, "Notebook", ct);
    }

    public async Task<Guid?> ResolveQueryIdByPathAsync(string path, CancellationToken ct)
    {
        return await ResolveItemIdByPathAsync(path, "Query", ct);
    }

    public async Task<string?> ResolveNotebookPathByIdAsync(Guid id, CancellationToken ct)
    {
        return await ResolveItemPathByIdAsync(id, "Notebook", ct);
    }

    public async Task<string?> ResolveQueryPathByIdAsync(Guid id, CancellationToken ct)
    {
        return await ResolveItemPathByIdAsync(id, "Query", ct);
    }

    private async Task<Guid?> ResolveItemIdByPathAsync(string path, string itemType, CancellationToken ct)
    {
        var normalized = path.Trim('/');
        var segments = normalized.Split('/');
        if (segments.Length == 0) return null;

        var itemTitle = segments[^1];
        var folderSegments = segments[..^1];

        var conn = await EnsureConnectionAsync(ct);

        // Walk the folder tree to find the target folder
        Guid? folderId = null;
        foreach (var folderName in folderSegments)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = folderId is null
                ? """SELECT "Id" FROM "Folders" WHERE "Name" = @name AND "ParentId" IS NULL"""
                : """SELECT "Id" FROM "Folders" WHERE "Name" = @name AND "ParentId" = @parentId""";

            AddParameter(cmd, "@name", folderName);
            if (folderId is not null)
                AddParameter(cmd, "@parentId", folderId.Value);

            var result = await cmd.ExecuteScalarAsync(ct);
            if (result is not Guid id)
                return null;

            folderId = id;
        }

        // Find the item in the resolved folder
        await using var itemCmd = conn.CreateCommand();
        itemCmd.CommandText = folderId is null
            ? """SELECT "Id" FROM "WorkspaceItems" WHERE "Title" = @title AND "ItemType" = @itemType AND "FolderId" IS NULL"""
            : """SELECT "Id" FROM "WorkspaceItems" WHERE "Title" = @title AND "ItemType" = @itemType AND "FolderId" = @folderId""";

        AddParameter(itemCmd, "@title", itemTitle);
        AddParameter(itemCmd, "@itemType", itemType);
        if (folderId is not null)
            AddParameter(itemCmd, "@folderId", folderId.Value);

        var itemResult = await itemCmd.ExecuteScalarAsync(ct);
        return itemResult as Guid?;
    }

    private async Task<string?> ResolveItemPathByIdAsync(Guid id, string itemType, CancellationToken ct)
    {
        var conn = await EnsureConnectionAsync(ct);

        // Get the item's title and folder ID
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT "Title", "FolderId"
            FROM "WorkspaceItems"
            WHERE "Id" = @id AND "ItemType" = @itemType
            """;
        AddParameter(cmd, "@id", id);
        AddParameter(cmd, "@itemType", itemType);

        string? title;
        Guid? folderId;
        await using (var reader = await cmd.ExecuteReaderAsync(ct))
        {
            if (!await reader.ReadAsync(ct)) return null;
            title = reader.GetString(0);
            folderId = reader.IsDBNull(1) ? null : reader.GetGuid(1);
        }

        // Walk up the folder tree to build the full path
        var pathParts = new List<string> { title };
        while (folderId is not null)
        {
            await using var folderCmd = conn.CreateCommand();
            folderCmd.CommandText = """SELECT "Name", "ParentId" FROM "Folders" WHERE "Id" = @id""";
            AddParameter(folderCmd, "@id", folderId.Value);

            await using var folderReader = await folderCmd.ExecuteReaderAsync(ct);
            if (!await folderReader.ReadAsync(ct)) break;

            pathParts.Add(folderReader.GetString(0));
            folderId = folderReader.IsDBNull(1) ? null : folderReader.GetGuid(1);
        }

        pathParts.Reverse();
        return "/" + string.Join("/", pathParts);
    }

    private async Task<WorkspaceItemContent?> ReadItemAsync(Guid id, string itemType, CancellationToken ct)
    {
        var conn = await EnsureConnectionAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT "Id", "Title", "Content"
            FROM "WorkspaceItems"
            WHERE "Id" = @id AND "ItemType" = @itemType
            """;

        AddParameter(cmd, "@id", id);
        AddParameter(cmd, "@itemType", itemType);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;

        return new WorkspaceItemContent(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.GetString(2));
    }

    private async Task<DbConnection> EnsureConnectionAsync(CancellationToken ct)
    {
        var conn = dbContext.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            await conn.OpenAsync(ct);
        return conn;
    }

    private static void AddParameter(DbCommand cmd, string name, object value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value;
        cmd.Parameters.Add(p);
    }
}
