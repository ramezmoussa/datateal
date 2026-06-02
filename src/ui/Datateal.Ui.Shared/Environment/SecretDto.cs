namespace Datateal.Ui.Shared.Environment;

/// <summary>
/// Secret DTO for listings. Value is never included.
/// </summary>
public record SecretDto(
    Guid Id,
    string Key,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateSecretRequest(
    string Key,
    string Value,
    string? Description = null);

public record UpdateSecretRequest(
    string Key,
    string? Value = null,
    string? Description = null);
