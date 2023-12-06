namespace WebApp.Services.Place;

using System.Threading;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Models;
using Location = Shared.Models.Location;
using Place = Shared.Models.Place;

public class PlacesService(IDbContextFactory<WorldExplorerDbContext> dbContextFactory, IOptions<PlacesSettings> placesOptions) : IPlacesService
{
	public async Task ClearPlaces(CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		await dbContext.Places.ExecuteDeleteAsync(cancellationToken);
	}

	public async Task<List<Place>> GetPlaces(CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		var dbPlaces = await dbContext.Places.ToListAsync(cancellationToken);
		return [.. dbPlaces.Select(ToPlace).OrderBy(x => x.Name)];
	}

	public async Task<OperationResult<List<Place>>> GetNearByPlaces(Location location, CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		var locationRequestNearbyDistance = placesOptions.Value.LocationRequestNearbyDistance;
		var userLocation = location.ToPoint();

		var locationInfoRequests = await dbContext.LocationInfoRequests.Where(x => x.Location.IsWithinDistance(userLocation, locationRequestNearbyDistance)).ToListAsync(cancellationToken);

		if (locationInfoRequests.Exists(x => x.Status != LocationInfoRequestStatus.Completed))
		{
			return new OperationResult<List<Place>>()
			{
				StatusCode = StatusCode.LocationInfoRequestPending
			};
		}

		var hasCompletedRequests = locationInfoRequests.Count > 0;
		if (hasCompletedRequests)
		{
			var nearbyDistance = placesOptions.Value.NearbyDistance;
			var nearestPlacesNearby = await dbContext.Places
													.Where(x => x.Location.IsWithinDistance(userLocation, nearbyDistance))
													.ToListAsync(cancellationToken);
			return new OperationResult<List<Place>>()
			{
				Result = nearestPlacesNearby.Select(ToPlace).ToList()
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

	public bool IsNearby(Location location1, Location location2)
	{
		var point1 = location1.ToPoint();
		var point2 = location2.ToPoint();
		return point1.IsWithinDistance(point2, placesOptions.Value.LocationDistance);
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
			Location = place.Location.ToLocation(),
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