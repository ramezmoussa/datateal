using DuckHouse.Ui.Server.Application.Ai;
using DuckHouse.Ui.Shared.Ai;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DuckHouse.Ui.Server.Hubs;

/// <summary>
/// SignalR hub for streaming AI assistant responses to the WASM client.
/// Ask mode: client calls <see cref="ChatAsync"/>; receives "ReceiveChunk" tokens + "StreamComplete".
/// Agent mode: client calls <see cref="AgentChatAsync"/>; receives "ReceiveChunk" tokens,
/// then "ReceiveProposals" (JSON list of <see cref="CellProposal"/>), then "StreamComplete".
/// On any error: "StreamError" is sent with a message.
/// </summary>
[Authorize]
public class AiAssistantHub(IAiChatService aiChatService, IAiAgentService aiAgentService) : Hub
{
    /// <summary>
    /// Initiates an AI chat completion stream (Ask mode).
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

    /// <summary>
    /// Initiates an agentic chat loop (Agent mode).
    /// The AI makes tool calls to propose cell edits; proposals are sent via "ReceiveProposals" when the loop completes.
    /// </summary>
    public async Task AgentChatAsync(AiChatRequest request)
    {
        try
        {
            await aiAgentService.StreamAgentChatAsync(
                request,
                onChunk: async chunk => await Clients.Caller.SendAsync("ReceiveChunk", chunk, Context.ConnectionAborted),
                onProposals: async proposals => await Clients.Caller.SendAsync("ReceiveProposals", proposals, Context.ConnectionAborted),
                cancellationToken: Context.ConnectionAborted);

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
