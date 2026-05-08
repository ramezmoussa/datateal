using Microsoft.Extensions.AI;
using DuckHouse.Ui.Shared.Ai;

namespace DuckHouse.Ui.Server.Application.Ai;

public interface IAiChatService
{
    /// <summary>
    /// Assembles context and streams an AI response token by token.
    /// </summary>
    IAsyncEnumerable<string> StreamChatAsync(AiChatRequest request, CancellationToken cancellationToken = default);
}
