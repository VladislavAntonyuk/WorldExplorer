namespace WorldExplorer.Modules.Travellers.Application.Travellers.GetTravellers;

using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

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