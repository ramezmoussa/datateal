using System.Text;
using DuckHouse.Ui.Shared.Ai;
using Microsoft.Extensions.AI;

namespace DuckHouse.Ui.Server.Application.Ai;

/// <summary>
/// Runs the AI agent loop: the AI calls the <c>propose_cell_edit</c> tool for each cell
/// it wants to modify. Proposals are collected server-side, then returned to the caller.
/// Text commentary from the AI is streamed chunk-by-chunk via <paramref name="onChunk"/>.
/// </summary>
public class AiAgentService(IContextAssembler contextAssembler, IAiProviderFactory providerFactory)
    : IAiAgentService
{
    public async Task StreamAgentChatAsync(
        AiChatRequest request,
        Func<string, Task> onChunk,
        Func<IReadOnlyList<CellProposal>, Task> onProposals,
        CancellationToken cancellationToken = default)
    {
        var proposals = new List<CellProposal>();

        var proposeCellEdit = AIFunctionFactory.Create(
            (int cellIndex, string newContent, string explanation) =>
            {
                proposals.Add(new CellProposal(cellIndex, newContent, explanation));
                return $"Proposal for cell {cellIndex} recorded.";
            },
            name: "propose_cell_edit",
            description: "Propose replacing the entire content of a notebook cell. Call this once per cell that needs to be modified.");

        var messages = await contextAssembler.AssembleAsync(request, cancellationToken);
        var baseClient = providerFactory.Create(request.Provider, request.ApiKey, request.Endpoint, request.Model);

        var agentClient = new FunctionInvokingChatClient(baseClient);
        var options = new ChatOptions
        {
            ModelId = request.Model,
            Tools = [proposeCellEdit],
        };

        var responseText = new StringBuilder();

        await foreach (var update in agentClient.GetStreamingResponseAsync(messages, options, cancellationToken))
        {
            var text = update.Text;
            if (!string.IsNullOrEmpty(text))
            {
                responseText.Append(text);
                await onChunk(text);
            }
        }

        await onProposals(proposals);
    }
}
