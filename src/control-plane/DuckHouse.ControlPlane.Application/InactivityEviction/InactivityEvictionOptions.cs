namespace DuckHouse.ControlPlane.Application.InactivityEviction;

public class InactivityEvictionOptions
{
    public bool Enabled { get; set; } = true;

    /// <summary>How long a kernel can be idle before it is deleted.</summary>
    public TimeSpan KernelIdleTimeout { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>How long a node can have no kernel activity before it is stopped.</summary>
    public TimeSpan NodeIdleTimeout { get; set; } = TimeSpan.FromMinutes(20);

    /// <summary>How often the eviction sweep runs.</summary>
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromMinutes(1);
}
