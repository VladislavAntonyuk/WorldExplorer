namespace WorldExplorer.Modules.Travellers;

using Database;
using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;

public sealed class TravellersService(TravellersDbContext context)
{
	[UseOffsetPaging]
	[UseFiltering]
	[UseSorting]
	public IQueryable<Traveller> GetTravellersAsync(CancellationToken ct = default)
		=> context.Travellers
		                .AsNoTracking()
		                .OrderBy(t => t.Id)
		                .ThenBy(t => t.Id);
}