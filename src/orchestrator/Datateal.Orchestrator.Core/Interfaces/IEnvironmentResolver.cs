namespace Datateal.Orchestrator.Core.Interfaces;

/// <summary>
/// Resolves environment variable and secret IDs into plaintext key-value pairs
/// by calling the UI server's resolve endpoint.
/// </summary>
public interface IEnvironmentResolver
{
    Task<ResolvedEnvironmentEntries> ResolveAsync(
        IReadOnlyList<Guid>? environmentVariableIds,
        IReadOnlyList<Guid>? secretIds,
        CancellationToken ct = default);
}

public record ResolvedEnvironmentEntries(
    IReadOnlyDictionary<string, string> Variables,
    IReadOnlyDictionary<string, string> Secrets);
