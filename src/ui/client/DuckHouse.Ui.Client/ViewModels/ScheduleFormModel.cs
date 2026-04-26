using DuckHouse.Ui.Client.Models;

namespace DuckHouse.Ui.Client.ViewModels;

public class ScheduleFormModel
{
    public string Cron { get; set; } = "";
    public string? TimeZone { get; set; }
    public List<EditableTaskParameter> Parameters { get; set; } = [];
}
