using Datateal.Core.Catalogs;

namespace Datateal.Orchestrator.Core.Interfaces;

/// <summary>
/// Resolves catalog names into decrypted connection details for kernel sessions.
/// </summary>
public interface ICatalogResolver
{
    Task<IReadOnlyList<ResolvedCatalog>> ResolveAsync(IReadOnlyList<string> catalogNames, CancellationToken ct = default);
}
