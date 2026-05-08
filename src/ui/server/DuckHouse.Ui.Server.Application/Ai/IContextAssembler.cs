using Microsoft.Extensions.AI;
using DuckHouse.Ui.Shared.Ai;

namespace DuckHouse.Ui.Server.Application.Ai;

public interface IContextAssembler
{
    /// <summary>
    /// Assembles a list of <see cref="ChatMessage"/> objects from the request context,
    /// including system prompt with DuckHouse-specific instructions, catalog schemas,
    /// package list, and the conversation history.
    /// </summary>
    Task<IReadOnlyList<ChatMessage>> AssembleAsync(AiChatRequest request, CancellationToken cancellationToken = default);
}
