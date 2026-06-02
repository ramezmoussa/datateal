using Datateal.Ui.Client.Models;

namespace Datateal.Ui.Client.ViewModels;

public class ScheduleFormModel
{
    public string Cron { get; set; } = "";
    public string? TimeZone { get; set; }
    public List<EditableTaskParameter> Parameters { get; set; } = [];
}
