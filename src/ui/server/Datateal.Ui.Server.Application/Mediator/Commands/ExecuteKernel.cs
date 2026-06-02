using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record ExecuteKernelRequest(string NodeName, string KernelId, string Code, double? Timeout = null) : IRequest<ExecutionHandle>;

internal class ExecuteKernelHandler(IKernelRepository kernelRepository) : IRequestHandler<ExecuteKernelRequest, ExecutionHandle>
{
    public Task<ExecutionHandle> Handle(ExecuteKernelRequest request, CancellationToken cancellationToken) =>
        kernelRepository.StartExecuteAsync(request.NodeName, request.KernelId, new ExecuteRequest(request.Code, request.Timeout), cancellationToken);
}
