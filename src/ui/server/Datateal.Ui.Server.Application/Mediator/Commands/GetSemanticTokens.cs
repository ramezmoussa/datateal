using Datateal.Core.Kernels;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Repositories;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record GetSemanticTokensRequest(string NodeName, string KernelId, SemanticTokenRequest Request) : IRequest<SemanticTokenResponse>;

internal class GetSemanticTokensHandler(IKernelRepository kernelRepository) : IRequestHandler<GetSemanticTokensRequest, SemanticTokenResponse>
{
    public Task<SemanticTokenResponse> Handle(GetSemanticTokensRequest request, CancellationToken cancellationToken) =>
        kernelRepository.GetSemanticTokensAsync(request.NodeName, request.KernelId, request.Request, cancellationToken);
}
