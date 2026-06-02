using Datateal.Ui.Shared.Ai;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Datateal.Ui.Client.Services;

/// <inheritdoc cref="IAiAssistantService"/>
public sealed class AiAssistantService : IAiAssistantService
{
    private HubConnection? _hub;
    private readonly NavigationManager _nav;

    public AiAssistantService(NavigationManager nav)
    {
        _nav = nav;
    }

    private async Task<HubConnection> GetOrCreateHubAsync(CancellationToken ct)
    {
        if (_hub is { State: HubConnectionState.Connected })
            return _hub;

        if (_hub is not null)
            await _hub.DisposeAsync();

        _hub = new HubConnectionBuilder()
            .WithUrl(_nav.ToAbsoluteUri("/ai/hub"))
            .WithAutomaticReconnect()
            .Build();

        await _hub.StartAsync(ct);
        return _hub;
    }

    public async Task StreamChatAsync(
        AiChatRequest request,
        Func<string, Task> onChunk,
        Func<Task> onComplete,
        Func<string, Task> onError,
        CancellationToken ct = default)
    {
        var hub = await GetOrCreateHubAsync(ct);

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        using var chunkSub = hub.On<string>("ReceiveChunk", async chunk => await onChunk(chunk));
        using var completeSub = hub.On("StreamComplete", async () =>
        {
            await onComplete();
            tcs.TrySetResult();
        });
        using var errorSub = hub.On<string>("StreamError", async error =>
        {
            await onError(error);
            tcs.TrySetResult();
        });

        using var ctReg = ct.Register(() => tcs.TrySetCanceled(ct));

        await hub.SendAsync("ChatAsync", request, ct);
        await tcs.Task;
    }

    public async Task StreamAgentChatAsync(
        AiChatRequest request,
        Func<string, Task> onChunk,
        Func<IReadOnlyList<CellProposal>, Task> onProposals,
        Func<Task> onComplete,
        Func<string, Task> onError,
        CancellationToken ct = default)
    {
        var hub = await GetOrCreateHubAsync(ct);

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        using var chunkSub = hub.On<string>("ReceiveChunk", async chunk => await onChunk(chunk));
        using var proposalsSub = hub.On<IReadOnlyList<CellProposal>>("ReceiveProposals",
            async proposals => await onProposals(proposals));
        using var completeSub = hub.On("StreamComplete", async () =>
        {
            await onComplete();
            tcs.TrySetResult();
        });
        using var errorSub = hub.On<string>("StreamError", async error =>
        {
            await onError(error);
            tcs.TrySetResult();
        });

        using var ctReg = ct.Register(() => tcs.TrySetCanceled(ct));

        await hub.SendAsync("AgentChatAsync", request, ct);
        await tcs.Task;
    }

    public async ValueTask DisposeAsync()
    {
        if (_hub is not null)
            await _hub.DisposeAsync();
    }
}
