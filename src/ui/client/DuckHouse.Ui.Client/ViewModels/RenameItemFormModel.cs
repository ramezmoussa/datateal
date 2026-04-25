using System.ComponentModel.DataAnnotations;

namespace DuckHouse.Ui.Client.ViewModels;

public class RenameItemFormModel
{
    [Required]
    public string Title { get; set; } = "";
}
