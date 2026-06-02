namespace Datateal.Orchestrator.Core.Configuration;

/// <summary>
/// Default connection settings for managed DuckLake catalogs.
/// Bound from the "Catalogs" configuration section.
/// Must stay in sync with <c>Datateal.Ui.Server.Core.Catalogs.CatalogSettings</c>.
/// </summary>
public class CatalogSettings
{
    public string BaseDataPath { get; set; } = string.Empty;
    public string? StorageConnectionString { get; set; }
    public string CatalogHost { get; set; } = "localhost";

    /// <summary>
    /// PostgreSQL host as seen from inside kernel pods.
    /// When set, overrides <see cref="CatalogHost"/> in DuckDB setup scripts executed by kernels.
    /// Useful for local development where the server reaches Postgres at "localhost" but pods
    /// must use "host.docker.internal". Defaults to <see cref="CatalogHost"/> when not set.
    /// </summary>
    public string? CatalogPodHost { get; set; }
    public int CatalogPort { get; set; } = 5432;
    public string CatalogUser { get; set; } = string.Empty;
    public string CatalogPassword { get; set; } = string.Empty;
}
