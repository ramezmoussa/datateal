namespace Datateal.Ui.Server.Core.Workspace;

/// <summary>
/// Thrown when a workspace folder or item name fails validation.
/// </summary>
public sealed class WorkspaceNameValidationException(string name, string reason)
    : InvalidOperationException($"The name \"{name}\" is not valid: {reason}")
{
    public string Name { get; } = name;

    /// <summary>
    /// Throws <see cref="WorkspaceNameValidationException"/> if <paramref name="name"/>
    /// contains a forward slash, which is reserved as the workspace path separator.
    /// </summary>
    public static void ValidateNoSlash(string name)
    {
        if (name.Contains('/'))
            throw new WorkspaceNameValidationException(name, "it cannot contain '/'.");
    }
}
