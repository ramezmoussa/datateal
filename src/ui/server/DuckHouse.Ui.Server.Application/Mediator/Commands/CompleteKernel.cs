using DuckHouse.Core.Kernels;
using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record CompleteKernelRequest(string NodeName, string KernelId, CompleteRequest Request) : IRequest<CompleteResponse>;

internal class CompleteKernelHandler(IKernelRepository kernelRepository) : IRequestHandler<CompleteKernelRequest, CompleteResponse>
{
    public Task<CompleteResponse> Handle(CompleteKernelRequest request, CancellationToken cancellationToken) =>
        kernelRepository.CompleteAsync(request.NodeName, request.KernelId, request.Request, cancellationToken);
}
