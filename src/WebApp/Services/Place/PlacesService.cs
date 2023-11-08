namespace WebApp.Services.Place;

using System.Threading;
using AI;
using Image;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Location = Shared.Models.Location;
using Place = Shared.Models.Place;

public class PlacesService(IDbContextFactory<WorldExplorerDbContext> dbContextFactory,
	IAiService aiService,
	IImageSearchService imageSearchService) : IPlacesService
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
		return dbPlaces.Select(ToPlace).OrderBy(x => x.Name).ToList();
	}

	public async Task<List<Place>> GetNearByPlaces(Location location, CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		var userLocation = new Point(location.Longitude, location.Latitude)
		{
			SRID = 4326
		};
		var nearestPlaces = await dbContext.Places.OrderBy(c => c.Location.Distance(userLocation)).Take(5).ToListAsync(cancellationToken);
		var nearestPlacesNearby = nearestPlaces.Where(x => x.Location.IsWithinDistance(userLocation, DistanceConstants.NearbyDistance)).ToList();
		if (nearestPlacesNearby.Count > 0)
		{
			return nearestPlacesNearby.Select(ToPlace).ToList();
		}

		// to do send request new places event
		await RequestNewPlaces(location, cancellationToken);
		return nearestPlaces.Select(ToPlace).ToList();
	}

	public async Task<Place?> GetPlaceDetails(string name, Location location, CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		var savedPlace = await dbContext.Places.FirstOrDefaultAsync(place => place.Name == name && place.Location.IsWithinDistance(new Point(location.Longitude, location.Latitude), DistanceConstants.NearbyDistance), cancellationToken);
		return savedPlace is null ? null : ToPlace(savedPlace);
	}

	public bool IsNearby(Location location1, Location location2, double distance)
	{
		var latLongDifferenceEquivalentToM = distance / DistanceConstants.MetersPerDegree;
		return location1.Latitude - location2.Latitude >= -latLongDifferenceEquivalentToM &&
			   location1.Latitude - location2.Latitude <= latLongDifferenceEquivalentToM &&
			   location1.Longitude - location2.Longitude >= -latLongDifferenceEquivalentToM &&
			   location1.Longitude - location2.Longitude <= latLongDifferenceEquivalentToM;
	}

	public async Task Delete(Guid placeId, CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		await dbContext.Places.Where(x => x.Id == placeId).ExecuteDeleteAsync(cancellationToken);
	}

	private async Task RequestNewPlaces(Location location, CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
		var places = await aiService.GetNearByPlaces(location);
		if (places.Count > 0)
		{
			var newPlaceNames = places.Select(x => x.Name);
			var existingPlaceNames = await dbContext.Places
													.Where(place => newPlaceNames.Contains(place.Name))
													.Select(place => place.Name)
													.ToListAsync(cancellationToken);

			var newPlaces = places.ExceptBy(existingPlaceNames, x => x.Name).ToList();

			var getImagesTasks = newPlaces.Select(async place =>
			{
				var images = await imageSearchService.GetPlaceImages(place.Name, cancellationToken);
				place.Images.AddRange(images);
			}).ToList();

			await Task.WhenAll(getImagesTasks);

			await dbContext.Places.AddRangeAsync(newPlaces.Select(ToPlace), cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}

	private static Infrastructure.Entities.Place ToPlace(Place place)
	{
		return new Infrastructure.Entities.Place
		{
			Name = place.Name,
			Description = place.Description,
			Images = place.Images.Select(x => new Image
			{
				Source = x
			}).ToList(),
			Location = new Point(place.Location.Longitude, place.Location.Latitude)
		};
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