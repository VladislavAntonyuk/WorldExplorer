namespace WorldExplorer.Modules.Places.Infrastructure.LocationInfo;

using Database;
using Domain.LocationInfo;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

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

	public async Task<LocationInfoRequest?> GetAsync(int id, CancellationToken cancellationToken = default)
	{
		return await placesDbContext.LocationInfoRequests.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

	public void Insert(LocationInfoRequest locationInfoRequest)
	{
		placesDbContext.LocationInfoRequests.Add(locationInfoRequest);
	}

	public async Task<List<LocationInfoRequest>> IsNearby(Point userLocation)
	{
		return await placesDbContext.LocationInfoRequests
		                            .Where(x => x.Location.IsWithinDistance(userLocation, 100))
		                            .ToListAsync();
	}
}