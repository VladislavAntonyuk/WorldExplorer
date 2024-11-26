namespace WorldExplorer.Modules.Travellers.Application.Visits.GetVisits;

using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

[ExtendObjectType("Travellers")]
public sealed class GetVisitsHandler(TravellersDbContext context)
{
	[UseOffsetPaging]
	[UseFiltering]
	[UseSorting]
	public IQueryable<Visit> GetVisitsByTravellerId(Guid travellerId, CancellationToken ct = default)
	{
		return context.Visits.AsNoTracking().Where(x=>x.TravellerId == travellerId).OrderBy(t => t.VisitDate).ThenBy(t => t.Id);
	}

	[UseOffsetPaging]
	[UseFiltering]
	[UseSorting]
	public IQueryable<Visit> GetVisitsByPlaceId(Guid placeId, CancellationToken ct = default)
	{
		return context.Visits.AsNoTracking().Where(x=>x.PlaceId == placeId).OrderBy(t => t.VisitDate).ThenBy(t => t.Id);
	}
}