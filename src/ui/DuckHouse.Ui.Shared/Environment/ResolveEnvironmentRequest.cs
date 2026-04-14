namespace DuckHouse.Ui.Shared.Environment;

/// <summary>
/// Request to resolve environment variable and secret IDs into key-value pairs.
/// Used internally during node creation.
/// </summary>
public record ResolveEnvironmentRequest(
    IReadOnlyList<Guid>? EnvironmentVariableIds = null,
    IReadOnlyList<Guid>? SecretIds = null);

/// <summary>
/// Resolved environment entries ready for injection into a container.
/// </summary>
public record ResolvedEnvironment(
    IReadOnlyDictionary<string, string> Variables,
    IReadOnlyDictionary<string, string> Secrets);
