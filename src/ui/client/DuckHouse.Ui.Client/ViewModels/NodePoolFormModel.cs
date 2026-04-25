using System.ComponentModel.DataAnnotations;

namespace DuckHouse.Ui.Client.ViewModels;

public class NodePoolFormModel
{
    [Required]
    public string Name { get; set; } = "";
    [Required]
    public string VmSize { get; set; } = "";
    public int KernelIdleMin { get; set; }
    public int? NodeIdleMin { get; set; }
    public string? KernelReqs { get; set; }
    public string? Description { get; set; }
    public int WarmNodes { get; set; }
    public int? MaxNodes { get; set; }
    public int? NodeAcquireTimeoutMin { get; set; }
    public IEnumerable<Guid> WheelPackageIds { get; set; } = [];
    public IEnumerable<Guid> EnvVarIds { get; set; } = [];
    public IEnumerable<Guid> SecretIds { get; set; } = [];
}
