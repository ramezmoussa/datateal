using DuckHouse.Ui.Shared.Ai;

namespace DuckHouse.Ui.Server.Application.Ai;

/// <summary>
/// Runs an agentic AI loop that proposes cell edits via tool calling.
/// </summary>
public interface IAiAgentService
{
    /// <summary>
    /// Runs the agent loop and streams back text chunks and collected cell proposals.
    /// </summary>
    /// <param name="request">The chat request (must have Mode = Agent and ContextType = Notebook).</param>
    /// <param name="onChunk">Called for each streamed text token.</param>
    /// <param name="onProposals">Called once when the agent loop completes, with all proposed cell edits.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StreamAgentChatAsync(
        AiChatRequest request,
        Func<string, Task> onChunk,
        Func<IReadOnlyList<CellProposal>, Task> onProposals,
        CancellationToken cancellationToken = default);
}
