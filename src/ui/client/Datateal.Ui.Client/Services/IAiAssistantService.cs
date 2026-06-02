using Datateal.Ui.Shared.Ai;

namespace Datateal.Ui.Client.Services;

/// <summary>
/// Client-side AI assistant service. Connects to the server SignalR hub
/// and streams AI chat responses token by token.
/// </summary>
public interface IAiAssistantService : IAsyncDisposable
{
    /// <summary>
    /// Sends a chat request (Ask mode) and streams back tokens via the provided callbacks.
    /// </summary>
    Task StreamChatAsync(
        AiChatRequest request,
        Func<string, Task> onChunk,
        Func<Task> onComplete,
        Func<string, Task> onError,
        CancellationToken ct = default);

    /// <summary>
    /// Sends a chat request (Agent mode) and streams back tokens and cell proposals via the provided callbacks.
    /// </summary>
    Task StreamAgentChatAsync(
        AiChatRequest request,
        Func<string, Task> onChunk,
        Func<IReadOnlyList<CellProposal>, Task> onProposals,
        Func<Task> onComplete,
        Func<string, Task> onError,
        CancellationToken ct = default);
}
