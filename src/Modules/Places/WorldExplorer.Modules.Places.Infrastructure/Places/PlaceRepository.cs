namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Database;
using Domain.Places;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

internal sealed class PlaceRepository(PlacesDbContext context) : IPlaceRepository
{
	public async Task<Place?> GetAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await context.Places.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public void Insert(Place place)
	{
		context.Places.Add(place);
	}

	public async Task<List<Place>> GetNearestPlacesAsync(Point userLocation, CancellationToken cancellationToken)
	{
		return await context.Places.Where(x => x.Location.IsWithinDistance(userLocation, 1000))
		                    .ToListAsync(cancellationToken);
	}
}