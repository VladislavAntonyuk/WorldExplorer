namespace WorldExplorer.Modules.Travellers.Application.Travellers.GetTravellers;

using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

[ExtendObjectType("Travellers")]
public sealed class GetTravellersHandler(TravellersDbContext context)
{
	[UsePaging]
	[UseFiltering]
	[UseSorting]
	public IQueryable<Traveller> GetTravellers(CancellationToken ct = default)
	{
		return context.Travellers
		              .AsNoTracking()
					  .Include(t => t.Visits)
		              .ThenInclude(x=> x.Review)
					  .OrderBy(t => t.Id);
	}
}