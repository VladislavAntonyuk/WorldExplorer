namespace WorldExplorer.Modules.Travellers.Application.Travellers.GetTravellers;

using Microsoft.EntityFrameworkCore;
using Travellers;
using WorldExplorer.Modules.Travellers.Infrastructure.Database;

[ExtendObjectType("Travellers")]
public sealed class GetTravellersHandler(TravellersDbContext context)
{
	[UseOffsetPaging]
	[UseFiltering]
	[UseSorting]
	public IQueryable<Traveller> GetTravellers(CancellationToken ct = default)
		=> context.Travellers
						.AsNoTracking()
						.OrderBy(t => t.Id)
						.ThenBy(t => t.Id);
}