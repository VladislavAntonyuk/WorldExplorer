namespace WebApp.Services.Place;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WorldExplorer.Modules.Places.Domain.LocationInfo;
using WorldExplorer.Modules.Places.Domain.Places;
using WorldExplorer.Modules.Places.Infrastructure.Database;
using Location = WorldExplorer.Modules.Places.Domain.Places.Location;

public class LocationInfoRepository(PlacesDbContext placesDbContext) : ILocationInfoRepository
{
	public async Task Clear(CancellationToken cancellationToken)
	{
		await placesDbContext.LocationInfoRequests.ExecuteDeleteAsync(cancellationToken);
	}

	public async Task<List<LocationInfoRequest>> GetRequests(CancellationToken cancellationToken)
	{
		var dbRequests = await placesDbContext.LocationInfoRequests.ToListAsync(cancellationToken);
		return dbRequests;
	}

	public async Task Delete(int requestId, CancellationToken cancellationToken)
	{
		await placesDbContext.LocationInfoRequests.Where(x => x.Id == requestId).ExecuteDeleteAsync(cancellationToken);
	}

	public Task<LocationInfoRequest?> GetAsync(Guid id, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public void Insert(LocationInfoRequest locationInfo)
	{
		throw new NotImplementedException();
	}

	public Task<List<LocationInfoRequest>> IsNearby(Location userLocation)
	{
		return placesDbContext.LocationInfoRequests
			.Where(x => new Point(x.Location.Longitude, x.Location.Latitude).IsWithinDistance(new Point(userLocation.Longitude, userLocation.Latitude), 100))
			.ToListAsync();
	}
}