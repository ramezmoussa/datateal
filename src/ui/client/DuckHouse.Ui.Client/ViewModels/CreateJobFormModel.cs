using System.ComponentModel.DataAnnotations;

namespace DuckHouse.Ui.Client.ViewModels;

public class CreateJobFormModel
{
    [Required]
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}
