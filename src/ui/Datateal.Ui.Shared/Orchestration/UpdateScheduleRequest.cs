namespace Datateal.Ui.Shared.Orchestration;

public record UpdateScheduleRequest(string CronExpression, bool IsEnabled, string? TimeZone, Dictionary<string, string>? Parameters);
