using Datateal.Core.Mediator;
using Datateal.Ui.Server.Core.Catalogs;
using Datateal.Ui.Server.Core.Repositories;
using Datateal.Ui.Shared.Catalogs;
using Microsoft.Extensions.Options;

namespace Datateal.Ui.Server.Application.Mediator.Queries;

public record GetCatalogsRequest : IRequest<IReadOnlyList<CatalogDto>>;

internal class GetCatalogsHandler(ICatalogRepository repository, IOptions<CatalogSettings> settings)
    : IRequestHandler<GetCatalogsRequest, IReadOnlyList<CatalogDto>>
{
    public async Task<IReadOnlyList<CatalogDto>> Handle(GetCatalogsRequest request, CancellationToken cancellationToken)
    {
        var catalogs = await repository.GetAllAsync(cancellationToken);
        var opts = settings.Value;
        return catalogs.Select(c => Commands.CatalogDtoMapper.ToDto(c, opts)).ToList();
    }
}
