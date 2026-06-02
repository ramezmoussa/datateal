using Datateal.Core.Catalogs;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Catalogs;
using Datateal.Ui.Server.Core.Repositories;
using Microsoft.Extensions.Options;

namespace Datateal.Ui.Server.Application.Mediator.Commands;

public record DeleteCatalogRequest(Guid Id) : IRequest<bool>;

internal class DeleteCatalogHandler(
    ICatalogRepository repository,
    ICatalogDatabaseService databaseService,
    IOptions<CatalogSettings> settings)
    : IRequestHandler<DeleteCatalogRequest, bool>
{
    public async Task<bool> Handle(DeleteCatalogRequest request, CancellationToken cancellationToken)
    {
        var catalog = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (catalog is null) return false;

        // Drop the PostgreSQL database for managed catalogs
        if (catalog is ManagedCatalog)
        {
            var opts = settings.Value;
            await databaseService.DropDatabaseAsync(
                catalog.Name,
                opts.CatalogHost,
                opts.CatalogPort,
                opts.CatalogUser,
                opts.CatalogPassword,
                cancellationToken);
        }

        return await repository.DeleteAsync(request.Id, cancellationToken);
    }
}
