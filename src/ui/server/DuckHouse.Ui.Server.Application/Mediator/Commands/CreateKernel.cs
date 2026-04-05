using DuckHouse.Core.Kernels;
using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Repositories;

namespace DuckHouse.Ui.Server.Application.Mediator.Commands;

public record CreateKernelRequest(string NodeName) : IRequest<KernelInfo>;

internal class CreateKernelHandler(IKernelRepository kernelRepository) : IRequestHandler<CreateKernelRequest, KernelInfo>
{
    public Task<KernelInfo> Handle(CreateKernelRequest request, CancellationToken cancellationToken) =>
        kernelRepository.CreateKernelAsync(request.NodeName, cancellationToken);
}
