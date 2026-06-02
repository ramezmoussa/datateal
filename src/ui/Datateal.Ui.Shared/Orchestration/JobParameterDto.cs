namespace Datateal.Ui.Shared.Orchestration;

public record JobParameterDto(Guid Id, string Name, string? DefaultValue, bool IsRequired, string? Description);
