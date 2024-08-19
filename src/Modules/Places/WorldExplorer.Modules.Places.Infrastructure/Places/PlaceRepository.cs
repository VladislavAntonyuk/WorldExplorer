namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Database;
using Domain.Places;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Location = Domain.Places.Location;

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

	public async Task<List<Place>> GetNearestPlacesAsync(Location userLocation, CancellationToken cancellationToken)
	{
		return await context.Places
		             .Where(x => new Point(x.Location.Longitude, x.Location.Latitude).IsWithinDistance(new Point(userLocation.Longitude, userLocation.Latitude), 1000))
		             .ToListAsync(cancellationToken);
	}
}