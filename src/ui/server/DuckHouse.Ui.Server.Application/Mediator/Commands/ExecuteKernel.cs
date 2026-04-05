using DuckHouse.Core.Kernels;
using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record ExecuteKernelRequest(string NodeName, string KernelId, string Code, double Timeout = 60.0) : IRequest<ExecutionResult>;

internal class ExecuteKernelHandler(IKernelRepository kernelRepository) : IRequestHandler<ExecuteKernelRequest, ExecutionResult>
{
    public Task<ExecutionResult> Handle(ExecuteKernelRequest request, CancellationToken cancellationToken) =>
        kernelRepository.ExecuteAsync(request.NodeName, request.KernelId, new ExecuteRequest(request.Code, request.Timeout), cancellationToken);
}
