using System.ComponentModel.DataAnnotations;

namespace DuckHouse.Ui.Client.ViewModels;

public class VariableFormModel
{
    [Required]
    public string Key { get; set; } = "";
    [Required]
    public string Value { get; set; } = "";
    public string? Description { get; set; }
}
