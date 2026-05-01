using DuckHouse.Auth;
using DuckHouse.Core.Catalogs;
using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Catalogs;
using DuckHouse.Ui.Shared.Catalogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cmd = DuckHouse.Ui.Server.Application.Mediator.Commands;
using Qry = DuckHouse.Ui.Server.Application.Mediator.Queries;
using SharedCat = DuckHouse.Ui.Shared.Catalogs;

namespace DuckHouse.Ui.Server.Controllers;

[ApiController]
[Route("api/catalogs")]
[Authorize]
public class CatalogsController(IMediator mediator, ICatalogAccessService catalogAccess) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<CatalogDto>> GetAll(CancellationToken ct)
    {
        var catalogs = await mediator.SendAsync(new Qry.GetCatalogsRequest(), ct);
        var accessibleIds = await catalogAccess.GetAccessibleCatalogIdsAsync(User, ct);
        if (accessibleIds is null)
            return catalogs;
        return catalogs.Where(c => accessibleIds.Contains(c.Id)).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        if (!await catalogAccess.HasAccessAsync(User, id, ct))
            return Forbid();
        var catalogs = await mediator.SendAsync(new Qry.GetCatalogsRequest(), ct);
        var catalog = catalogs.FirstOrDefault(c => c.Id == id);
        return catalog is null ? NotFound() : Ok(catalog);
    }

    [HttpPost("managed")]
    [Authorize(Policy = AuthPolicy.CatalogManage)]
    public async Task<IActionResult> CreateManaged(SharedCat.CreateManagedCatalogRequest body, CancellationToken ct)
    {
        var catalog = await mediator.SendAsync(new Cmd.CreateManagedCatalogCommand(body.Name, body.AllowExistingDatabase), ct);
        return Created($"api/catalogs/{catalog.Id}", catalog);
    }

    [HttpPost("unmanaged")]
    [Authorize(Policy = AuthPolicy.CatalogManage)]
    public async Task<IActionResult> CreateUnmanaged(SharedCat.CreateUnmanagedCatalogRequest body, CancellationToken ct)
    {
        var catalog = await mediator.SendAsync(
            new Cmd.CreateUnmanagedCatalogCommand(
                body.Name, body.DataPath, body.StorageConnectionString,
                body.CatalogHost, body.CatalogPort, body.CatalogDatabase,
                body.CatalogUser, body.CatalogPassword), ct);
        return Created($"api/catalogs/{catalog.Id}", catalog);
    }

    [HttpPut("{id:guid}/managed")]
    [Authorize(Policy = AuthPolicy.CatalogManage)]
    public async Task<IActionResult> UpdateManaged(Guid id, SharedCat.UpdateManagedCatalogRequest body, CancellationToken ct)
    {
        var catalog = await mediator.SendAsync(new Cmd.UpdateManagedCatalogCommand(id, body.Name), ct);
        return catalog is null ? NotFound() : Ok(catalog);
    }

    [HttpPut("{id:guid}/unmanaged")]
    [Authorize(Policy = AuthPolicy.CatalogManage)]
    public async Task<IActionResult> UpdateUnmanaged(Guid id, SharedCat.UpdateUnmanagedCatalogRequest body, CancellationToken ct)
    {
        var catalog = await mediator.SendAsync(
            new Cmd.UpdateUnmanagedCatalogCommand(
                id, body.Name, body.DataPath, body.StorageConnectionString,
                body.CatalogHost, body.CatalogPort, body.CatalogDatabase,
                body.CatalogUser, body.CatalogPassword), ct);
        return catalog is null ? NotFound() : Ok(catalog);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthPolicy.CatalogManage)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await mediator.SendAsync(new Cmd.DeleteCatalogRequest(id), ct);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}/metadata")]
    public async Task<IActionResult> GetMetadata(Guid id, CancellationToken ct)
    {
        if (!await catalogAccess.HasAccessAsync(User, id, ct))
            return Forbid();
        var metadata = await mediator.SendAsync(new Qry.GetCatalogMetadataRequest(id), ct);
        return metadata is null ? NotFound() : Ok(metadata);
    }

    [HttpGet("{id:guid}/info")]
    public async Task<IActionResult> GetInfo(Guid id, CancellationToken ct)
    {
        if (!await catalogAccess.HasAccessAsync(User, id, ct))
            return Forbid();
        var info = await mediator.SendAsync(new Qry.GetCatalogInfoRequest(id), ct);
        return info is null ? NotFound() : Ok(info);
    }
}
