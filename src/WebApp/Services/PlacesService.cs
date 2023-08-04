namespace WebApp.Services;

using System.Linq;
using System.Linq.Expressions;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Location = global::Shared.Models.Location;
using Place = global::Shared.Models.Place;

public static class DistanceConstants
{
	public const double MetersPerDegree = 111139; // Approximate for both latitude and longitude
	public const double SettlementDistance = 20000;
	public const double NearbyDistance = 2000;
	public const double LocationDistance = 100;
}

public interface IPlacesService
{
	Task<List<Place>> GetPlaces(CancellationToken cancellationToken);
	Task<List<Place>> GetNearByPlaces(Location location, CancellationToken cancellationToken);
	Task<Place?> GetPlaceDetails(string name, Location location, CancellationToken cancellationToken);
	Task ClearPlaces(CancellationToken cancellationToken);
	bool IsNearby(Location location1, Location location2, double distance);
}

public class PlacesService : IPlacesService
{
	private readonly IAiService aiService;
	private readonly IDbContextFactory<WorldExplorerDbContext> dbContextFactory;
	private readonly IImageSearchService imageSearchService;

	public PlacesService(IDbContextFactory<WorldExplorerDbContext> dbContextFactory,
		IAiService aiService,
		IImageSearchService imageSearchService)
	{
		this.dbContextFactory = dbContextFactory;
		this.aiService = aiService;
		this.imageSearchService = imageSearchService;
	}

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
		var result = new List<Place>();
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

		var inSettlementLocation = IsNearbyLocation(location, DistanceConstants.SettlementDistance);
		var savedPlaces = await dbContext.Places.Where(inSettlementLocation).ToListAsync(cancellationToken);
		if (savedPlaces.Count == 0)
		{
			var places = await aiService.GetNearByPlaces(location);
			if (places.Count > 0)
			{
				var newPlaces = places.ExceptBy(savedPlaces.Select(x => x.Name), x => x.Name).ToList();
				foreach (var place in newPlaces)
				{
					place.Images.AddRange(await imageSearchService.GetPlaceImages(place.Name, cancellationToken));
				}

				await dbContext.Places.AddRangeAsync(newPlaces.Select(ToPlace), cancellationToken);
				await dbContext.SaveChangesAsync(cancellationToken);
				result.AddRange(newPlaces);
			}
		}
		else
		{
			result.AddRange(savedPlaces.Select(ToPlace));
		}

		return result.Where(place => IsNearby(location, place.Location, DistanceConstants.NearbyDistance)).ToList();
	}

	public async Task<Place?> GetPlaceDetails(string name, Location location, CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

		var nearbyCondition = IsNearbyLocation(location, DistanceConstants.NearbyDistance);
		Expression<Func<Infrastructure.Models.Place, bool>> nameCondition = place => place.Name == name;

		var nearbyAndNameCondition = Expression.Lambda<Func<Infrastructure.Models.Place, bool>>(
			Expression.AndAlso(nearbyCondition.Body, Expression.Invoke(nameCondition, nearbyCondition.Parameters)),
			nearbyCondition.Parameters);
		var savedPlace = await dbContext.Places.FirstOrDefaultAsync(nearbyAndNameCondition, cancellationToken);
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

	private Expression<Func<Infrastructure.Models.Place, bool>> IsNearbyLocation(Location location1, double distance)
	{
		var latLongDifferenceEquivalentToM = distance / DistanceConstants.MetersPerDegree;
		return place => location1.Latitude - place.Location.Latitude >= -latLongDifferenceEquivalentToM &&
		                location1.Latitude - place.Location.Latitude <= latLongDifferenceEquivalentToM &&
		                location1.Longitude - place.Location.Longitude >= -latLongDifferenceEquivalentToM &&
		                location1.Longitude - place.Location.Longitude <= latLongDifferenceEquivalentToM;
	}

	private Infrastructure.Models.Place ToPlace(Place place)
	{
		return new Infrastructure.Models.Place
		{
			Name = place.Name,
			Description = place.Description,
			Images = place.Images.Select(x => new Image
			              {
				              Source = x
			              })
			              .ToList(),
			Location = new Infrastructure.Models.Location
			{
				Latitude = place.Location.Latitude,
				Longitude = place.Location.Longitude
			}
		};
	}

	private Place ToPlace(Infrastructure.Models.Place place)
	{
		return new Place
		{
			Name = place.Name,
			Description = place.Description,
			Images = place.Images.Select(x => x.Source).ToList(),
			Location = new Location(place.Location.Latitude, place.Location.Longitude)
		};
	}
}