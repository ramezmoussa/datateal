using DuckHouse.Ui.Shared.Ai;

namespace DuckHouse.Ui.Client.Services;

/// <summary>
/// Client-side AI assistant service. Connects to the server SignalR hub
/// and streams AI chat responses token by token.
/// </summary>
public interface IAiAssistantService : IAsyncDisposable
{
    /// <summary>
    /// Sends a chat request and streams back tokens via the provided callbacks.
    /// </summary>
    /// <param name="request">The request including provider, API key, messages, and context.</param>
    /// <param name="onChunk">Called with each streamed text token.</param>
    /// <param name="onComplete">Called when streaming is complete.</param>
    /// <param name="onError">Called with an error message if streaming fails.</param>
    Task StreamChatAsync(
        AiChatRequest request,
        Func<string, Task> onChunk,
        Func<Task> onComplete,
        Func<string, Task> onError,
        CancellationToken ct = default);
}
