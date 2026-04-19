using System.Text.RegularExpressions;

namespace DuckHouse.Ui.Client.Validation;

public static partial class ValidationHelper
{
    /// <summary>
    /// Maximum length for a node pool name. Matches the AKS node pool name limit,
    /// which is the binding constraint (more restrictive than the 63-character Kubernetes limit).
    /// </summary>
    public const int MaxNodePoolNameLength = 12;

    // Kubernetes DNS label: lowercase alphanumeric and hyphens, start/end with alphanumeric, max 63 chars.
    [GeneratedRegex(@"^[a-z0-9]([a-z0-9\-]{0,61}[a-z0-9])?$")]
    private static partial Regex KubernetesNameRegex();

    // AKS node pool name: lowercase letters and digits only, 1-12 chars, must start with a letter.
    [GeneratedRegex(@"^[a-z][a-z0-9]{0,11}$")]
    private static partial Regex AksNodePoolNameRegex();

    // Python identifier: letter or underscore, followed by letters, digits, or underscores.
    [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]*$")]
    private static partial Regex PythonIdentifierRegex();

    // Environment variable key: letters, digits, and underscores; must start with a letter or underscore.
    [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]*$")]
    private static partial Regex EnvVarKeyRegex();

    // Basic cron field: *, number, */N, N-M, N,M,... or combinations.
    [GeneratedRegex(@"^(\*|(\d+(-\d+)?(,\d+(-\d+)?)*)|(\*/\d+))$")]
    private static partial Regex CronFieldRegex();

    private static readonly HashSet<string> PythonKeywords = new(StringComparer.Ordinal)
    {
        "False", "None", "True", "and", "as", "assert", "async", "await",
        "break", "class", "continue", "def", "del", "elif", "else", "except",
        "finally", "for", "from", "global", "if", "import", "in", "is",
        "lambda", "nonlocal", "not", "or", "pass", "raise", "return",
        "try", "while", "with", "yield"
    };

    /// <summary>
    /// Validates a Kubernetes resource name (DNS label rules).
    /// Returns an error message, or null if valid.
    /// </summary>
    public static string? ValidateKubernetesName(string? name)
    {
        if (string.IsNullOrEmpty(name))
            return null; // empty is handled by "required" checks elsewhere

        if (name.Length > 63)
            return "Name must be 63 characters or fewer.";

        if (!KubernetesNameRegex().IsMatch(name))
        {
            if (name != name.ToLowerInvariant())
                return "Name must be lowercase.";
            if (name.StartsWith('-') || name.EndsWith('-'))
                return "Name must start and end with a letter or digit.";
            return "Name may only contain lowercase letters, digits, and hyphens.";
        }

        return null;
    }

    /// <summary>
    /// Validates a node name. Combines Kubernetes DNS label rules with AKS node pool name rules:
    /// only lowercase letters and digits (no hyphens), must start with a letter, max 12 characters.
    /// Returns an error message, or null if valid.
    /// </summary>
    public static string? ValidateNodeName(string? name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        if (name.Length > 12)
            return "Name must be 12 characters or fewer (AKS node pool name limit).";

        if (!AksNodePoolNameRegex().IsMatch(name))
        {
            if (name != name.ToLowerInvariant())
                return "Name must be lowercase.";
            if (!char.IsLetter(name[0]))
                return "Name must start with a letter.";
            return "Name may only contain lowercase letters and digits (no hyphens).";
        }

        return null;
    }

    /// <summary>
    /// Validates a node pool name. Combines Kubernetes DNS label rules with AKS node pool name rules:
    /// only lowercase letters and digits (no hyphens), must start with a letter, max 12 characters.
    /// The AKS 12-character limit is the binding constraint (more restrictive than the
    /// Kubernetes-derived <see cref="MaxNodePoolNameLength"/> of 50 characters).
    /// Returns an error message, or null if valid.
    /// </summary>
    public static string? ValidateNodePoolName(string? name) => ValidateNodeName(name);

    /// <summary>
    /// Validates a Python identifier (variable/parameter name).
    /// Returns an error message, or null if valid.
    /// </summary>
    public static string? ValidatePythonIdentifier(string? name)
    {
        if (string.IsNullOrEmpty(name))
            return null; // empty is handled by "required" checks elsewhere

        if (!PythonIdentifierRegex().IsMatch(name))
            return "Must start with a letter or underscore, followed by letters, digits, or underscores.";

        if (PythonKeywords.Contains(name))
            return $"'{name}' is a reserved Python keyword.";

        return null;
    }

    /// <summary>
    /// Validates a TimeSpan string (e.g. "00:00:30").
    /// Returns an error message, or null if valid.
    /// </summary>
    public static string? ValidateTimeSpan(string? value, bool required)
    {
        if (string.IsNullOrWhiteSpace(value))
            return required ? "This field is required." : null;

        return TimeSpan.TryParse(value, out _)
            ? null
            : "Enter a valid duration (e.g. 00:01:00 or 01:30:00).";
    }

    /// <summary>
    /// Validates a 5-field cron expression.
    /// Returns an error message, or null if valid.
    /// </summary>
    public static string? ValidateCronExpression(string? cron)
    {
        if (string.IsNullOrWhiteSpace(cron))
            return null; // required check handled by button Disabled

        var fields = cron.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (fields.Length != 5)
            return "Cron expression must have exactly 5 fields (minute hour day month weekday).";

        foreach (var field in fields)
        {
            if (!CronFieldRegex().IsMatch(field))
                return $"Invalid cron field: '{field}'. Use *, a number, N-M, N,M,... or */N.";
        }

        return null;
    }

    /// <summary>
    /// Validates an environment variable key.
    /// Returns an error message, or null if valid.
    /// </summary>
    public static string? ValidateEnvVarKey(string? key)
    {
        if (string.IsNullOrEmpty(key))
            return null;

        if (key.Length > 256)
            return "Key must be 256 characters or fewer.";

        if (!EnvVarKeyRegex().IsMatch(key))
        {
            if (char.IsDigit(key[0]))
                return "Key must not start with a digit.";
            return "Key may only contain letters, digits, and underscores.";
        }

        return null;
    }

    // DuckDB/Postgres catalog identifier: letter or underscore, followed by letters, digits, or underscores.
    [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]*$")]
    private static partial Regex CatalogIdentifierRegex();

    /// <summary>
    /// Validates a catalog name. Must be a valid DuckDB identifier and a valid PostgreSQL database name:
    /// letters, digits, and underscores only; cannot start with a digit; max 63 characters (PostgreSQL limit);
    /// cannot start with "pg_" (reserved for PostgreSQL system catalogs).
    /// Returns an error message, or null if valid.
    /// </summary>
    public static string? ValidateCatalogName(string? name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        if (name.Length > 63)
            return "Catalog name must be 63 characters or fewer (PostgreSQL database name limit).";

        if (!CatalogIdentifierRegex().IsMatch(name))
        {
            if (char.IsDigit(name[0]))
                return "Catalog name must not start with a digit.";
            return "Catalog name may only contain letters, digits, and underscores.";
        }

        if (name.StartsWith("pg_", StringComparison.OrdinalIgnoreCase))
            return "Catalog name must not start with \"pg_\" (reserved for PostgreSQL system catalogs).";

        return null;
    }
}
