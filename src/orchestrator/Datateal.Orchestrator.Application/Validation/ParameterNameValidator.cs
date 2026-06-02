using System.Text.RegularExpressions;

namespace Datateal.Orchestrator.Application.Validation;

/// <summary>
/// Validates that job parameter names are valid Python identifiers.
/// This prevents code injection when parameter names are emitted as Python variable assignments.
/// </summary>
public static partial class ParameterNameValidator
{
    [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]*$")]
    private static partial Regex PythonIdentifierRegex();

    /// <summary>
    /// Validates that <paramref name="name"/> is a valid Python identifier
    /// (letters, digits, and underscores only; cannot start with a digit).
    /// Throws <see cref="InvalidOperationException"/> if validation fails.
    /// </summary>
    public static void Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Parameter name is required.");

        if (!PythonIdentifierRegex().IsMatch(name))
            throw new InvalidOperationException(
                $"Parameter name \"{name}\" is not valid. " +
                "Names must contain only letters, digits, and underscores, and cannot start with a digit.");
    }
}
