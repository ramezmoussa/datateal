namespace Datateal.Ui.Shared.Orchestration;

public record CreateScheduleRequest(string CronExpression, bool IsEnabled, string? TimeZone, Dictionary<string, string>? Parameters);
