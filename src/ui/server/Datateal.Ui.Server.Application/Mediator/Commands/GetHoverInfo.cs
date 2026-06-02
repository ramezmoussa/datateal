using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record GetHoverInfoRequest(string NodeName, string KernelId, HoverInfoRequest Request) : IRequest<HoverInfoResponse>;

internal class GetHoverInfoHandler(IKernelRepository kernelRepository) : IRequestHandler<GetHoverInfoRequest, HoverInfoResponse>
{
    public Task<HoverInfoResponse> Handle(GetHoverInfoRequest request, CancellationToken cancellationToken) =>
        kernelRepository.GetHoverInfoAsync(request.NodeName, request.KernelId, request.Request, cancellationToken);
}
