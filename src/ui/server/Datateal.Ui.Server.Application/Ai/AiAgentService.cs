using System.Text;
using Datateal.Ui.Shared.Ai;
using Microsoft.Extensions.AI;

namespace Datateal.Ui.Server.Application.Ai;

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
                proposals.Add(new CellProposal(CellProposalOperation.Edit, cellIndex, newContent, explanation));
                return $"Edit proposal for cell {cellIndex} recorded.";
            },
            name: "propose_cell_edit",
            description: "Propose replacing the entire content of an existing notebook cell. Call once per cell to modify.");

        var proposeCellInsert = AIFunctionFactory.Create(
            (int afterCellIndex, string language, string content, string explanation) =>
            {
                proposals.Add(new CellProposal(CellProposalOperation.Insert, afterCellIndex, content, explanation, language));
                return $"Insert proposal after cell {afterCellIndex} recorded.";
            },
            name: "propose_cell_insert",
            description: "Propose inserting a new cell into the notebook. afterCellIndex is the 0-based index of the cell the new cell should follow; use -1 to insert before the first cell. language must be 'python', 'sql', or 'markdown'.");

        var proposeCellRemove = AIFunctionFactory.Create(
            (int cellIndex, string explanation) =>
            {
                proposals.Add(new CellProposal(CellProposalOperation.Remove, cellIndex, null, explanation));
                return $"Remove proposal for cell {cellIndex} recorded.";
            },
            name: "propose_cell_remove",
            description: "Propose removing an existing notebook cell entirely.");

        var messages = await contextAssembler.AssembleAsync(request, cancellationToken);
        var baseClient = providerFactory.Create(request.Provider, request.ApiKey, request.Endpoint, request.Model);

        var agentClient = new FunctionInvokingChatClient(baseClient);
        var options = new ChatOptions
        {
            ModelId = request.Model,
            Tools = [proposeCellEdit, proposeCellInsert, proposeCellRemove],
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
