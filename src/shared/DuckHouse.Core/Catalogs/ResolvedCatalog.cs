namespace DuckHouse.Core.Catalogs;

/// <summary>
/// Fully resolved (decrypted) catalog connection details for attaching a DuckLake catalog to a kernel session.
/// </summary>
public record ResolvedCatalog(
    string Name,
    string DataPath,
    string? StorageConnectionString,
    string CatalogHost,
    int CatalogPort,
    string CatalogDatabase,
    string CatalogUser,
    string CatalogPassword);
