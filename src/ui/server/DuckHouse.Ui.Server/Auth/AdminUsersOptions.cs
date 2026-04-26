namespace DuckHouse.Ui.Server.Auth;

/// <summary>
/// Configuration for the admin seed list. Users listed here
/// automatically receive the Admin role without needing a database record.
/// </summary>
public class AdminUsersOptions
{
    public List<string> AdminUsers { get; set; } = [];
}
