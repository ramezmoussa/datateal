namespace DuckHouse.Orchestrator.Core.Entities;

public abstract class NodePoolConfig
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string PoolType { get; set; } = string.Empty;
    public required string VmSize { get; set; }
    public TimeSpan? KernelIdleTimeout { get; set; }
    public TimeSpan? NodeIdleTimeout { get; set; }
    public string? KernelRequirements { get; set; }
    public string? Description { get; set; }
    public List<Guid>? WheelPackageIds { get; set; }
    public List<Guid>? EnvironmentVariableIds { get; set; }
    public List<Guid>? SecretIds { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
