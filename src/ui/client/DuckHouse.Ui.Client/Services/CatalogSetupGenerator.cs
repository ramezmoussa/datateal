using System.Text;
using DuckHouse.Ui.Shared.Catalogs;

namespace DuckHouse.Ui.Client.Services;

/// <summary>
/// Generates DuckDB setup commands for attaching DuckLake catalogs to a kernel session.
/// </summary>
public static class CatalogSetupGenerator
{
    /// <summary>
    /// Generates the complete setup script for the given resolved catalogs as Python code
    /// that runs DuckDB commands via <c>duckdb.execute()</c>, matching the kernel's Python environment.
    /// </summary>
    public static string GenerateSetupScript(IReadOnlyList<ResolvedCatalogDto> catalogs, bool isLinux = false)
    {
        if (catalogs.Count == 0) return string.Empty;

        var sb = new StringBuilder();

        sb.AppendLine("import duckdb");
        sb.AppendLine("duckdb.execute(\"INSTALL ducklake\")");
        sb.AppendLine("duckdb.execute(\"LOAD ducklake\")");

        var anyAzure = catalogs.Any(c => c.StorageConnectionString is not null);
        if (anyAzure)
        {
            sb.AppendLine("duckdb.execute(\"INSTALL azure\")");
            sb.AppendLine("duckdb.execute(\"LOAD azure\")");

            if (isLinux)
                sb.AppendLine("duckdb.execute(\"SET azure_transport_option_type = 'curl'\")");
        }

        for (var i = 0; i < catalogs.Count; i++)
        {
            var catalog = catalogs[i];
            var suffix = catalogs.Count > 1 ? $"_{catalog.Name}" : "";

            if (catalog.StorageConnectionString is not null)
            {
                var azureSecret = $"CREATE SECRET secret{suffix}_storage (" +
                    $"TYPE azure, " +
                    $"CONNECTION_STRING '{EscapeSql(catalog.StorageConnectionString)}', " +
                    $"SCOPE '{GetAzureScope(catalog.DataPath)}'" +
                    $")";
                sb.AppendLine($"duckdb.execute(\"\"\"{EscapePython(azureSecret)}\"\"\")");
            }

            var pgSecret = $"CREATE SECRET secret{suffix}_pg (" +
                $"TYPE postgres, " +
                $"HOST '{EscapeSql(catalog.CatalogHost)}', " +
                $"PORT {catalog.CatalogPort}, " +
                $"DATABASE '{EscapeSql(catalog.CatalogDatabase)}', " +
                $"USER '{EscapeSql(catalog.CatalogUser)}', " +
                $"PASSWORD '{EscapeSql(catalog.CatalogPassword)}', " +
                $"SCOPE 'postgres://{EscapeSql(catalog.CatalogHost)}:{catalog.CatalogPort}/{EscapeSql(catalog.CatalogDatabase)}'" +
                $")";
            sb.AppendLine($"duckdb.execute(\"\"\"{EscapePython(pgSecret)}\"\"\")");

            sb.AppendLine(
                $"duckdb.execute(\"ATTACH 'ducklake:postgres:' AS {catalog.Name} " +
                $"(DATA_PATH '{EscapeSql(catalog.DataPath)}', META_SECRET 'secret{suffix}_pg', AUTOMATIC_MIGRATION true)\")");
        }

        return sb.ToString();
    }

    private static string EscapeSql(string value) => value.Replace("'", "''");

    /// <summary>Escapes backslashes and triple double-quotes so values are safe inside Python triple-quoted strings.</summary>
    private static string EscapePython(string value) => value.Replace("\\", "\\\\").Replace("\"\"\"", "\\\"\\\"\\\"");

    private static string GetAzureScope(string dataPath)
    {
        // Extract the container-level scope from the data path
        // e.g., abfss://container@account.dfs.core.windows.net/path → az://account.dfs.core.windows.net/container
        if (dataPath.StartsWith("abfss://", StringComparison.OrdinalIgnoreCase) ||
            dataPath.StartsWith("az://", StringComparison.OrdinalIgnoreCase))
        {
            return dataPath;
        }

        return "az://";
    }
}
