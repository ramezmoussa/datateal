using DuckHouse.Ui.Server.Application.Ai;
using DuckHouse.Ui.Shared.Ai;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DuckHouse.Ui.Server.Hubs;

/// <summary>
/// SignalR hub for streaming AI assistant responses to the WASM client.
/// The client calls <see cref="ChatAsync"/> and receives streamed chunks via
/// "ReceiveChunk" and a final "StreamComplete" or "StreamError" notification.
/// </summary>
[Authorize]
public class AiAssistantHub(IAiChatService aiChatService) : Hub
{
    /// <summary>
    /// Initiates an AI chat completion stream.
    /// Tokens are sent back to the caller via "ReceiveChunk".
    /// On completion, "StreamComplete" is sent.
    /// On error, "StreamError" is sent with an error message.
    /// </summary>
    public async Task ChatAsync(AiChatRequest request)
    {
        try
        {
            await foreach (var chunk in aiChatService.StreamChatAsync(request, Context.ConnectionAborted))
            {
                await Clients.Caller.SendAsync("ReceiveChunk", chunk, Context.ConnectionAborted);
            }

            await Clients.Caller.SendAsync("StreamComplete", Context.ConnectionAborted);
        }
        catch (OperationCanceledException)
        {
            // Client disconnected — nothing to do
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("StreamError", ex.Message, Context.ConnectionAborted);
        }
    }
}
