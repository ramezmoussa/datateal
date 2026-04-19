using System.Text.RegularExpressions;

namespace DuckHouse.Orchestrator.Application.Validation;

/// <summary>
/// Validates names that must satisfy both Kubernetes DNS label rules and AKS node pool name rules.
/// Combined constraint: only lowercase letters and digits, start with a letter, max 12 characters.
/// </summary>
public static partial class NodeNameValidator
{
    // AKS node pool name: lowercase letters and digits only, 1-12 chars, must start with a letter.
    [GeneratedRegex(@"^[a-z][a-z0-9]{0,11}$")]
    private static partial Regex AksNodePoolNameRegex();

    /// <summary>
    /// Validates a node pool configuration name against the combined Kubernetes and AKS node pool
    /// name rules: all lowercase, 1–12 characters, must start with a letter, only letters and digits.
    /// Returns an error message, or null if valid.
    /// </summary>
    public static string? ValidateNodePoolName(string? name)
    {
        if (string.IsNullOrEmpty(name))
            return "Node pool name is required.";

        if (name.Length > 12)
            return "Node pool name must be 12 characters or fewer (AKS node pool name limit).";

        if (!AksNodePoolNameRegex().IsMatch(name))
        {
            if (name != name.ToLowerInvariant())
                return "Node pool name must be lowercase.";
            if (!char.IsLetter(name[0]))
                return "Node pool name must start with a letter.";
            return "Node pool name may only contain lowercase letters and digits (no hyphens).";
        }

        return null;
    }
}
