namespace Datateal.Ui.Client.ViewModels;

public class ParameterFormModel
{
    public string Name { get; set; } = "";
    public string? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public string? Description { get; set; }
}
