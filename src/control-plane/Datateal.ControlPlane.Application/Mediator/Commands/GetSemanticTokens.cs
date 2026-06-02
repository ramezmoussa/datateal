using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.ControlPlane.Core.Services;

namespace Datateal.ControlPlane.Application.Mediator.Commands;

public record GetSemanticTokensRequest(string NodeName, string KernelId, SemanticTokenRequest Request) : IRequest<SemanticTokenResponse>;

internal class GetSemanticTokensHandler(INodeRuntimeClient runtimeClient) : IRequestHandler<GetSemanticTokensRequest, SemanticTokenResponse>
{
    public Task<SemanticTokenResponse> Handle(GetSemanticTokensRequest request, CancellationToken cancellationToken) =>
        runtimeClient.GetSemanticTokensAsync(request.NodeName, request.KernelId, request.Request, cancellationToken);
}
