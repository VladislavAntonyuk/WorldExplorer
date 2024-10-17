namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Database;
using Domain.Places;
using LocationInfo;
using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using WorldExplorer.Modules.Places.Application.Places.GetPlace;

internal sealed class PlaceRepository(PlacesDbContext context, IOptions<PlacesSettings> placeOptions) : IPlaceRepository
{
	public async Task<Place?> GetAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await context.Places.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<List<Place>> GetAsync(CancellationToken cancellationToken = default)
	{
		return await context.Places.ToListAsync(cancellationToken);
	}

	public void Insert(Place place)
	{
		context.Places.Add(place);
	}

	public async Task<List<Place>> GetNearestPlacesAsync(Point userLocation, CancellationToken cancellationToken)
	{
		return await context.Places
							.Where(x => x.Location.IsWithinDistance(userLocation, placeOptions.Value.LocationRequestNearbyDistance))
							.ToListAsync(cancellationToken);
	}

	public async Task Delete(Guid placeRequestId, CancellationToken cancellationToken)
	{
		await context.Places.Where(x => x.Id == placeRequestId).ExecuteDeleteAsync(cancellationToken);
	}

	public async Task Clear(CancellationToken cancellationToken)
	{
		await context.Places.ExecuteDeleteAsync(cancellationToken);
	}
}