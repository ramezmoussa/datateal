namespace Datateal.Ui.Shared.Environment;

public record EnvironmentVariableDto(
    Guid Id,
    string Key,
    string Value,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateEnvironmentVariableRequest(
    string Key,
    string Value,
    string? Description = null);

public record UpdateEnvironmentVariableRequest(
    string Key,
    string Value,
    string? Description = null);
