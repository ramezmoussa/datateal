using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record CreateKernelRequest(string NodeName) : IRequest<KernelInfo>;

internal class CreateKernelHandler(IKernelRepository kernelRepository) : IRequestHandler<CreateKernelRequest, KernelInfo>
{
    public Task<KernelInfo> Handle(CreateKernelRequest request, CancellationToken cancellationToken) =>
        kernelRepository.CreateKernelAsync(request.NodeName, cancellationToken);
}
