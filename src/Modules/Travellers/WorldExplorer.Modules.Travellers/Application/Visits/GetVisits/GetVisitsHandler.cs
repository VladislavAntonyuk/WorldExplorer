﻿namespace WorldExplorer.Modules.Travellers.Application.Visits.GetVisits;

using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

[ExtendObjectType("Travellers")]
public sealed class GetVisitsHandler(TravellersDbContext context)
{
	[UsePaging]
	[UseFiltering]
	[UseSorting]
	public IQueryable<Visit> GetVisitsByTravellerId(Guid travellerId, CancellationToken ct = default)
	{
		return context.Visits
					  .AsNoTracking()
					  .Include(x => x.Review)
					  .Where(x => x.TravellerId == travellerId)
					  .OrderBy(t => t.VisitDate)
					  .ThenBy(t => t.Id);
	}

	[UsePaging]
	[UseFiltering]
	[UseSorting]
	public IQueryable<Visit> GetVisitsByPlaceId(Guid placeId, CancellationToken ct = default)
	{
		return context.Visits
					  .AsNoTracking()
					  .Include(x => x.Review)
					  .Include(x => x.Traveller)
					  .Where(x => x.PlaceId == placeId)
					  .OrderBy(t => t.VisitDate)
					  .ThenBy(t => t.Id);
	}
}