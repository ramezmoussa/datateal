using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record CompleteKernelRequest(string NodeName, string KernelId, CompleteRequest Request) : IRequest<CompleteResponse>;

internal class CompleteKernelHandler(IKernelRepository kernelRepository) : IRequestHandler<CompleteKernelRequest, CompleteResponse>
{
    public Task<CompleteResponse> Handle(CompleteKernelRequest request, CancellationToken cancellationToken) =>
        kernelRepository.CompleteAsync(request.NodeName, request.KernelId, request.Request, cancellationToken);
}
