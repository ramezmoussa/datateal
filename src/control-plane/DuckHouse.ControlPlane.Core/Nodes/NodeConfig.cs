namespace DuckHouse.ControlPlane.Core.Nodes;

public class NodeConfig
{
    public string NodeName { get; set; } = "";

    /// <summary>
    /// How long a kernel can be idle before it is deleted.
    /// <c>null</c> = use the global default; <c>TimeSpan.Zero</c> = never evict kernels.
    /// </summary>
    public TimeSpan? KernelIdleTimeout { get; set; }

    /// <summary>
    /// How long a node can be idle (no kernels) before it is stopped.
    /// <c>null</c> = use the global default; <c>TimeSpan.Zero</c> = never evict this node.
    /// </summary>
    public TimeSpan? NodeIdleTimeout { get; set; }
}
