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

    private async Task<WorkspaceItemContent?> ReadItemAsync(Guid id, string itemType, CancellationToken ct)
    {
        var conn = dbContext.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT "Id", "Title", "Content"
            FROM "WorkspaceItems"
            WHERE "Id" = @id AND "ItemType" = @itemType
            """;

        var idParam = cmd.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = id;
        cmd.Parameters.Add(idParam);

        var typeParam = cmd.CreateParameter();
        typeParam.ParameterName = "@itemType";
        typeParam.Value = itemType;
        cmd.Parameters.Add(typeParam);

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;

        return new WorkspaceItemContent(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.GetString(2));
    }
}
