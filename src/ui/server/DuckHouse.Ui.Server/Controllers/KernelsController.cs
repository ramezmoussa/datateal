using DuckHouse.Core.Kernels;
using DuckHouse.Core.Mediator;
using Microsoft.AspNetCore.Mvc;
using Cmd = DuckHouse.Ui.Server.Application.Mediator.Commands;
using Qry = DuckHouse.Ui.Server.Application.Mediator.Queries;
using SharedKernels = DuckHouse.Ui.Shared.Kernels;

namespace DuckHouse.Ui.Server.Controllers;

[ApiController]
[Route("api/nodes/{nodeName}/kernels")]
public class KernelsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<KernelInfo>> GetKernels(string nodeName, CancellationToken ct) =>
        await mediator.SendAsync(new Qry.GetKernelsRequest(nodeName), ct);

    [HttpPost]
    public async Task<IActionResult> CreateKernel(string nodeName, CancellationToken ct)
    {
        var kernel = await mediator.SendAsync(new Cmd.CreateKernelRequest(nodeName), ct);
        return CreatedAtAction(nameof(GetKernel), new { nodeName, kernelId = kernel.Id }, kernel);
    }

    [HttpGet("{kernelId}")]
    public async Task<KernelInfo> GetKernel(string nodeName, string kernelId, CancellationToken ct) =>
        await mediator.SendAsync(new Qry.GetKernelRequest(nodeName, kernelId), ct);

    [HttpDelete("{kernelId}")]
    public async Task<IActionResult> DeleteKernel(string nodeName, string kernelId, CancellationToken ct)
    {
        await mediator.SendAsync(new Cmd.DeleteKernelRequest(nodeName, kernelId), ct);
        return NoContent();
    }

    [HttpPost("{kernelId}/execute")]
    public async Task<ExecutionResult> Execute(string nodeName, string kernelId, SharedKernels.ExecuteKernelRequest body, CancellationToken ct) =>
        await mediator.SendAsync(new Cmd.ExecuteKernelRequest(nodeName, kernelId, body.Code, body.Timeout), ct);

    [HttpPost("{kernelId}/restart")]
    public async Task<KernelInfo> RestartKernel(string nodeName, string kernelId, CancellationToken ct) =>
        await mediator.SendAsync(new Cmd.RestartKernelRequest(nodeName, kernelId), ct);

    [HttpPost("{kernelId}/interrupt")]
    public async Task<IActionResult> InterruptKernel(string nodeName, string kernelId, CancellationToken ct)
    {
        await mediator.SendAsync(new Cmd.InterruptKernelRequest(nodeName, kernelId), ct);
        return NoContent();
    }
}
