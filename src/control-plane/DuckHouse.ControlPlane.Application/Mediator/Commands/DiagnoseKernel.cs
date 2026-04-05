using DuckHouse.Core.Kernels;
using DuckHouse.Core.Mediator;
using DuckHouse.ControlPlane.Core.Services;

namespace DuckHouse.ControlPlane.Application.Mediator.Commands;

public record DiagnoseKernelRequest(string NodeName, string KernelId, DiagnoseRequest Request) : IRequest<DiagnoseResponse>;

internal class DiagnoseKernelHandler(INodeRuntimeClient runtimeClient) : IRequestHandler<DiagnoseKernelRequest, DiagnoseResponse>
{
    public Task<DiagnoseResponse> Handle(DiagnoseKernelRequest request, CancellationToken cancellationToken) =>
        runtimeClient.DiagnoseAsync(request.NodeName, request.KernelId, request.Request, cancellationToken);
}
