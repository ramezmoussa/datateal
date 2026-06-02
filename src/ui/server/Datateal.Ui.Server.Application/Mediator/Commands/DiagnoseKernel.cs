using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record DiagnoseKernelRequest(string NodeName, string KernelId, DiagnoseRequest Request) : IRequest<DiagnoseResponse>;

internal class DiagnoseKernelHandler(IKernelRepository kernelRepository) : IRequestHandler<DiagnoseKernelRequest, DiagnoseResponse>
{
    public Task<DiagnoseResponse> Handle(DiagnoseKernelRequest request, CancellationToken cancellationToken) =>
        kernelRepository.DiagnoseAsync(request.NodeName, request.KernelId, request.Request, cancellationToken);
}
