using System.Text.RegularExpressions;

namespace DuckHouse.Core.Catalogs;

/// <summary>
/// Abstract base for all catalog variants.
/// Must be a valid DuckDB database name/alias (alphanumeric + underscores, no leading digit).
/// </summary>
public abstract class Catalog
{
    private static readonly Regex ValidIdentifier = new("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

    public Guid Id { get; set; }

    public CatalogType CatalogType { get; protected set; }

    /// <summary>
    /// Must be a valid DuckDB identifier and PostgreSQL database name.
    /// Unique across all catalogs.
    /// </summary>
    public required string Name { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static bool IsValidName(string name) => ValidIdentifier.IsMatch(name);

    public static void ValidateName(string name)
    {
        if (!IsValidName(name))
            throw new ArgumentException($"Catalog name '{name}' is not a valid identifier. Names must match [a-zA-Z_][a-zA-Z0-9_]*.", nameof(name));
    }
}
