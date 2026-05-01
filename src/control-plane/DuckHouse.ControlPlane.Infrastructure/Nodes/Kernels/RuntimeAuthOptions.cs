namespace DuckHouse.ControlPlane.Infrastructure.Nodes.Kernels;

public class RuntimeAuthOptions
{
    public const string Section = "ServiceAuth:Runtime";

    /// <summary>
    /// API key sent as X-Api-Key header to the runtime pods.
    /// When empty or not configured, requests are sent without an API key (backward-compatible).
    /// </summary>
    public string ApiKey { get; set; } = "";
}
