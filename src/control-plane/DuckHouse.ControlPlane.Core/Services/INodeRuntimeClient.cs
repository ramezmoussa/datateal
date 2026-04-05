using DuckHouse.Core.Kernels;

namespace DuckHouse.ControlPlane.Core.Services;

public interface INodeRuntimeClient
{
    Task<IReadOnlyList<KernelInfo>> ListKernelsAsync(string nodeName, CancellationToken cancellationToken = default);
    Task<KernelInfo> CreateKernelAsync(string nodeName, CancellationToken cancellationToken = default);
    Task<KernelInfo> GetKernelAsync(string nodeName, string kernelId, CancellationToken cancellationToken = default);
    Task DeleteKernelAsync(string nodeName, string kernelId, CancellationToken cancellationToken = default);
    Task<ExecutionResult> ExecuteAsync(string nodeName, string kernelId, ExecuteRequest request, CancellationToken cancellationToken = default);
    Task<KernelInfo> RestartKernelAsync(string nodeName, string kernelId, CancellationToken cancellationToken = default);
    Task InterruptKernelAsync(string nodeName, string kernelId, CancellationToken cancellationToken = default);
}
