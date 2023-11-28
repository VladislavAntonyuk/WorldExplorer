namespace WebApp.Services.Place;

using System.Threading;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Shared.Enums;
using Shared.Models;
using Location = Shared.Models.Location;
using Place = Shared.Models.Place;

public class PlacesService(IDbContextFactory<WorldExplorerDbContext> dbContextFactory) : IPlacesService
{
	public async Task ClearPlaces(CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		await dbContext.Places.ExecuteDeleteAsync(cancellationToken);
		await dbContext.LocationInfoRequests.ExecuteDeleteAsync(cancellationToken);
	}
	public async Task ClearRequests(CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		await dbContext.LocationInfoRequests.ExecuteDeleteAsync(cancellationToken);
	}

	public async Task<List<Place>> GetPlaces(CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		var dbPlaces = await dbContext.Places.ToListAsync(cancellationToken);
		return dbPlaces.Select(ToPlace).OrderBy(x => x.Name).ToList();
	}

	public async Task<OperationResult<List<Place>>> GetNearByPlaces(Location location, CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		var userLocation = location.ToPoint();

		var locationInfoRequests = await dbContext.LocationInfoRequests.Where(x => x.Location.IsWithinDistance(userLocation, DistanceConstants.NearbyDistance)).ToListAsync(cancellationToken);

		if (locationInfoRequests.Exists(x => x.Status != LocationInfoRequestStatus.Completed))
		{
			return new OperationResult<List<Place>>()
			{
				StatusCode = StatusCode.LocationInfoRequestPending
			};
		}

		var nearestPlacesNearby = await dbContext.Places
												 .Where(x => x.Location.IsWithinDistance(userLocation, DistanceConstants.NearbyDistance))
												 .OrderBy(c => c.Location.Distance(userLocation))
												 .ToListAsync(cancellationToken);
		if (nearestPlacesNearby.Count > 0)
		{
			return new OperationResult<List<Place>>()
			{
				Result = nearestPlacesNearby.Select(ToPlace).ToList()
			};
		}

		if (locationInfoRequests.Count > 0)
		{
			return new OperationResult<List<Place>>()
			{
				Result = []
			};
		}

		var locationInfoRequest = new LocationInfoRequest()
		{
			Status = LocationInfoRequestStatus.New,
			Location = userLocation,
			CreationDate = DateTime.UtcNow
		};
		await dbContext.LocationInfoRequests.AddAsync(locationInfoRequest, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
		return new OperationResult<List<Place>>()
		{
			StatusCode = StatusCode.LocationInfoRequestPending
		};
	}

	public async Task<Place?> GetPlaceDetails(Guid id, CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		var savedPlace = await dbContext.Places.FirstOrDefaultAsync(place => place.Id == id, cancellationToken);
		return savedPlace is null ? null : ToPlace(savedPlace);
	}

	public bool IsNearby(Location location1, Location location2, double distance)
	{
		var point1 = new Point(location1.Longitude, location1.Latitude);
		var point2 = new Point(location2.Longitude, location2.Latitude);
		return point1.IsWithinDistance(point2, distance);
	}

	public async Task Delete(Guid placeId, CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		await dbContext.Places.Where(x => x.Id == placeId).ExecuteDeleteAsync(cancellationToken);
	}

	private static Place ToPlace(Infrastructure.Entities.Place place)
	{
		return new Place
		{
			Id = place.Id,
			Name = place.Name,
			Description = place.Description,
			Images = place.Images.Select(x => x.Source).ToList(),
			Location = new Location(place.Location.Y, place.Location.X),
			Reviews = place.Reviews.Select(x => new Shared.Models.Review
			{
				Id = x.Id,
				Comment = x.Comment,
				Rating = x.Rating,
				ReviewDate = x.ReviewDate
			}).ToList()
		};
	}
}