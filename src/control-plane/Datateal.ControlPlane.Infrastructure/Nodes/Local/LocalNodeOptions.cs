namespace Datateal.ControlPlane.Infrastructure.Nodes.Local;

public class LocalNodeOptions
{
    public const string Section = "NodeService:Local";

    /// <summary>
    /// The kubeconfig context to use. Defaults to "docker-desktop".
    /// Set to null to use the current-context from the kubeconfig file.
    /// </summary>
    public string? KubeContext { get; set; } = "docker-desktop";

    /// <summary>
    /// Host path (as seen from the Docker Desktop WSL2 VM) to mount into every created pod.
    /// For a Windows path <c>C:\path\to\data</c>, use <c>/run/desktop/mnt/host/c/path/to/data</c>.
    /// Must be set together with <see cref="DataVolumeMountPath"/> to take effect.
    /// Leave empty for production or when persistence is not required.
    /// </summary>
    public string? DataVolumeHostPath { get; set; }

    /// <summary>
    /// Path inside the container at which <see cref="DataVolumeHostPath"/> is mounted.
    /// Set <c>Catalogs:BaseDataPath</c> to the same value so DuckLake data is written here.
    /// Must be set together with <see cref="DataVolumeHostPath"/> to take effect.
    /// </summary>
    public string? DataVolumeMountPath { get; set; }
}
