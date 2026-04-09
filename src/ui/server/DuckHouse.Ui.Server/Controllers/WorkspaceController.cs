using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Shared.Workspace;
using Microsoft.AspNetCore.Mvc;
using Cmd = DuckHouse.Ui.Server.Application.Mediator.Commands;
using Qry = DuckHouse.Ui.Server.Application.Mediator.Queries;
using SharedWorkspace = DuckHouse.Ui.Shared.Workspace;

namespace DuckHouse.Ui.Server.Controllers;

[ApiController]
[Route("api/workspace")]
public class WorkspaceController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<WorkspaceListing> GetRoot(CancellationToken ct) =>
        await mediator.SendAsync(new Qry.GetWorkspaceRequest(), ct);

    [HttpGet("folders/{id:guid}")]
    public async Task<WorkspaceListing> GetFolder(Guid id, CancellationToken ct) =>
        await mediator.SendAsync(new Qry.GetWorkspaceRequest(id), ct);

    [HttpPost("folders")]
    public async Task<IActionResult> CreateFolder(SharedWorkspace.CreateFolderRequest body, CancellationToken ct)
    {
        var folder = await mediator.SendAsync(new Cmd.CreateFolderRequest(body.Name, body.ParentId), ct);
        return CreatedAtAction(nameof(GetFolder), new { id = folder.Id }, folder);
    }

    [HttpPut("folders/{id:guid}")]
    public async Task<IActionResult> UpdateFolder(Guid id, SharedWorkspace.UpdateFolderRequest body, CancellationToken ct)
    {
        var folder = await mediator.SendAsync(new Cmd.UpdateFolderRequest(id, body.Name, body.ParentId), ct);
        return folder is null ? NotFound() : Ok(folder);
    }

    [HttpDelete("folders/{id:guid}")]
    public async Task<IActionResult> DeleteFolder(Guid id, CancellationToken ct)
    {
        var found = await mediator.SendAsync(new Cmd.DeleteFolderRequest(id), ct);
        return found ? NoContent() : NotFound();
    }

    [HttpGet("notebooks/{id:guid}")]
    public async Task<IActionResult> GetNotebook(Guid id, CancellationToken ct)
    {
        var notebook = await mediator.SendAsync(new Qry.GetNotebookRequest(id), ct);
        return notebook is null ? NotFound() : Ok(notebook);
    }

    [HttpPost("notebooks")]
    public async Task<IActionResult> CreateNotebook(SharedWorkspace.CreateNotebookRequest body, CancellationToken ct)
    {
        var notebook = await mediator.SendAsync(new Cmd.CreateNotebookRequest(body.Title, body.Content, body.FolderId), ct);
        return CreatedAtAction(nameof(GetNotebook), new { id = notebook.Id }, notebook);
    }

    [HttpPut("notebooks/{id:guid}")]
    public async Task<IActionResult> UpdateNotebook(Guid id, SharedWorkspace.UpdateNotebookRequest body, CancellationToken ct)
    {
        var notebook = await mediator.SendAsync(new Cmd.UpdateNotebookRequest(id, body.Title, body.Content, body.FolderId), ct);
        return notebook is null ? NotFound() : Ok(notebook);
    }

    [HttpDelete("notebooks/{id:guid}")]
    public async Task<IActionResult> DeleteNotebook(Guid id, CancellationToken ct)
    {
        var found = await mediator.SendAsync(new Cmd.DeleteNotebookRequest(id), ct);
        return found ? NoContent() : NotFound();
    }
}
