using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.ControlPlane.Core.Services;

namespace Datateal.ControlPlane.Application.Mediator.Commands;

public record ExecuteKernelRequest(string NodeName, string KernelId, string Code, double? Timeout = null) : IRequest<ExecutionHandle>;

internal class ExecuteKernelHandler(INodeRuntimeClient runtimeClient) : IRequestHandler<ExecuteKernelRequest, ExecutionHandle>
{
    public Task<ExecutionHandle> Handle(ExecuteKernelRequest request, CancellationToken cancellationToken) =>
        runtimeClient.StartExecuteAsync(request.NodeName, request.KernelId, new ExecuteRequest(request.Code, request.Timeout), cancellationToken);
}
