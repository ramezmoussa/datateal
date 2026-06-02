using Datateal.Core.Catalogs;
using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Catalogs;
using Datateal.Ui.Server.Core.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Datateal.Ui.Server.Application.Mediator.Queries;

public record ResolveCatalogsRequest(IReadOnlyList<string> CatalogNames) : IRequest<IReadOnlyList<ResolvedCatalog>>;

internal class ResolveCatalogsHandler(
    ICatalogRepository repository,
    IDataProtectionProvider dataProtection,
    IOptions<CatalogSettings> settings)
    : IRequestHandler<ResolveCatalogsRequest, IReadOnlyList<ResolvedCatalog>>
{
    private readonly IDataProtector _protector = dataProtection.CreateProtector("Datateal.Catalogs");

    public async Task<IReadOnlyList<ResolvedCatalog>> Handle(ResolveCatalogsRequest request, CancellationToken cancellationToken)
    {
        if (request.CatalogNames.Count == 0) return [];

        var catalogs = await repository.GetByNamesAsync(request.CatalogNames, cancellationToken);
        var opts = settings.Value;

        return catalogs.Select(c =>
        {
            if (c is ManagedCatalog)
            {
                return new ResolvedCatalog(
                    c.Name,
                    DataPath: opts.BaseDataPath.TrimEnd('/') + "/" + c.Name,
                    StorageConnectionString: !string.IsNullOrEmpty(opts.StorageConnectionString)
                        ? opts.StorageConnectionString
                        : null,
                    CatalogHost: !string.IsNullOrEmpty(opts.CatalogPodHost)
                        ? opts.CatalogPodHost
                        : opts.CatalogHost,
                    CatalogPort: opts.CatalogPort,
                    CatalogDatabase: c.Name,
                    CatalogUser: opts.CatalogUser,
                    CatalogPassword: opts.CatalogPassword);
            }

            var u = (UnmanagedCatalog)c;
            return new ResolvedCatalog(
                u.Name,
                DataPath: u.DataPath,
                StorageConnectionString: u.EncryptedStorageConnectionString is not null
                    ? _protector.Unprotect(u.EncryptedStorageConnectionString)
                    : null,
                CatalogHost: u.CatalogHost,
                CatalogPort: u.CatalogPort,
                CatalogDatabase: u.CatalogDatabase,
                CatalogUser: u.CatalogUser,
                CatalogPassword: u.EncryptedCatalogPassword is not null
                    ? _protector.Unprotect(u.EncryptedCatalogPassword)
                    : string.Empty);
        }).ToList();
    }
}
