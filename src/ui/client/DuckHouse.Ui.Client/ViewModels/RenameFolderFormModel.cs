using System.ComponentModel.DataAnnotations;

namespace DuckHouse.Ui.Client.ViewModels;

public class RenameFolderFormModel
{
    [Required]
    public string Name { get; set; } = "";
}
