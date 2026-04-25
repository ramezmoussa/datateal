using DuckHouse.Core.Catalogs;
using DuckHouse.Core.Mediator;
using DuckHouse.Ui.Server.Core.Catalogs;
using DuckHouse.Ui.Server.Core.Repositories;
using DuckHouse.Ui.Shared.Catalogs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace DuckHouse.Ui.Server.Application.Mediator.Queries;

public record GetCatalogInfoRequest(Guid CatalogId) : IRequest<CatalogInfoDto?>;

internal class GetCatalogInfoHandler(
    ICatalogRepository repository,
    ICatalogMetadataService metadataService,
    IDataProtectionProvider dataProtection,
    IOptions<CatalogSettings> settings)
    : IRequestHandler<GetCatalogInfoRequest, CatalogInfoDto?>
{
    private readonly IDataProtector _protector = dataProtection.CreateProtector("DuckHouse.Catalogs");

    public async Task<CatalogInfoDto?> Handle(GetCatalogInfoRequest request, CancellationToken cancellationToken)
    {
        var catalog = await repository.GetByIdAsync(request.CatalogId, cancellationToken);
        if (catalog is null) return null;

        string catalogHost, catalogDatabase, catalogUser, catalogPassword;
        int catalogPort;

        if (catalog is ManagedCatalog)
        {
            var opts = settings.Value;
            catalogHost = opts.CatalogHost;
            catalogPort = opts.CatalogPort;
            catalogDatabase = catalog.Name;
            catalogUser = opts.CatalogUser;
            catalogPassword = opts.CatalogPassword;
        }
        else
        {
            var u = (UnmanagedCatalog)catalog;
            catalogHost = u.CatalogHost;
            catalogPort = u.CatalogPort;
            catalogDatabase = u.CatalogDatabase;
            catalogUser = u.CatalogUser;
            catalogPassword = u.EncryptedCatalogPassword is not null
                ? _protector.Unprotect(u.EncryptedCatalogPassword)
                : string.Empty;
        }

        var result = await metadataService.GetCatalogInfoAsync(
            catalogHost, catalogPort, catalogDatabase, catalogUser, catalogPassword, cancellationToken);

        return new CatalogInfoDto(
            result.Metadata.Select(e =>
                new CatalogMetadataEntryDto(e.Key, e.Value, e.Scope, e.ScopeId))
                .ToList());
    }
}
