using DuckHouse.Orchestrator.Core.Interfaces;
using DuckHouse.Orchestrator.Infrastructure.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace DuckHouse.Orchestrator.Infrastructure.Repositories;

/// <summary>
/// Resolves environment variable and secret IDs by reading directly from the shared
/// UI database. Secrets are decrypted using the shared Data Protection key ring.
/// </summary>
internal class EnvironmentResolver(
    OrchestratorDbContext dbContext,
    IDataProtectionProvider dataProtectionProvider) : IEnvironmentResolver
{
    private const string DataProtectionPurpose = "DuckHouse.Secrets";

    public async Task<ResolvedEnvironmentEntries> ResolveAsync(
        IReadOnlyList<Guid>? environmentVariableIds,
        IReadOnlyList<Guid>? secretIds,
        CancellationToken ct = default)
    {
        var variables = new Dictionary<string, string>();
        var secrets = new Dictionary<string, string>();

        if (environmentVariableIds is { Count: > 0 })
        {
            var conn = dbContext.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync(ct);

            foreach (var id in environmentVariableIds)
            {
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = """SELECT "Key", "Value" FROM "EnvironmentVariables" WHERE "Id" = @id""";
                var p = cmd.CreateParameter();
                p.ParameterName = "@id";
                p.Value = id;
                cmd.Parameters.Add(p);

                await using var reader = await cmd.ExecuteReaderAsync(ct);
                if (await reader.ReadAsync(ct))
                {
                    variables[reader.GetString(0)] = reader.GetString(1);
                }
            }
        }

        if (secretIds is { Count: > 0 })
        {
            var protector = dataProtectionProvider.CreateProtector(DataProtectionPurpose);
            var conn = dbContext.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync(ct);

            foreach (var id in secretIds)
            {
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = """SELECT "Key", "EncryptedValue" FROM "Secrets" WHERE "Id" = @id""";
                var p = cmd.CreateParameter();
                p.ParameterName = "@id";
                p.Value = id;
                cmd.Parameters.Add(p);

                await using var reader = await cmd.ExecuteReaderAsync(ct);
                if (await reader.ReadAsync(ct))
                {
                    var key = reader.GetString(0);
                    var encryptedValue = reader.GetString(1);
                    secrets[key] = protector.Unprotect(encryptedValue);
                }
            }
        }

        return new ResolvedEnvironmentEntries(variables, secrets);
    }
}
