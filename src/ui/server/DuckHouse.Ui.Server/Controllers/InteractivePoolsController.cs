using DuckHouse.Auth;
using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Shared.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cmd = DuckHouse.Ui.Server.Application.Mediator.Commands;
using Qry = DuckHouse.Ui.Server.Application.Mediator.Queries;

namespace DuckHouse.Ui.Server.Controllers;

[ApiController]
[Authorize(Policy = AuthPolicy.NodePoolOperate)]
[Route("api/interactive-pools")]
public class InteractivePoolsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<InteractivePoolDto>> GetInteractivePools(CancellationToken ct) =>
        await mediator.SendAsync(new Qry.GetInteractivePoolsRequest(), ct);

    [HttpPost("{name}/ensure-node")]
    public async Task<IActionResult> EnsureNode(string name, CancellationToken ct)
    {
        var node = await mediator.SendAsync(new Cmd.EnsureInteractiveNodeRequest(name), ct);
        return node is null ? NotFound() : Ok(node);
    }
}
