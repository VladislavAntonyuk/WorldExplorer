namespace WebApp.Services;

using System.Linq.Expressions;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Location = global::Shared.Models.Location;
using Place = global::Shared.Models.Place;

public interface IPlacesService
{
	Task<List<Place>> GetNearByPlaces(Location location, CancellationToken cancellationToken);
	Task<Place?> GetPlaceDetails(string name, Location location, CancellationToken cancellationToken);
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

	public async Task<List<Place>> GetNearByPlaces(Location location, CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

		var nearbyCondition = IsNearbyLocation(location);
		var savedPlaces = await dbContext.Places.Where(nearbyCondition).ToListAsync(cancellationToken);
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
				return places;
			}
		}

		return savedPlaces.Select(ToPlace).ToList();
	}

	public async Task<Place?> GetPlaceDetails(string name, Location location, CancellationToken cancellationToken)
	{
		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

		var nearbyCondition = IsNearbyLocation(location);
		Expression<Func<Infrastructure.Models.Place, bool>> nameCondition = place => place.Name == name;

		var nearbyAndNameCondition = Expression.Lambda<Func<Infrastructure.Models.Place, bool>>(
			Expression.AndAlso(nearbyCondition.Body, Expression.Invoke(nameCondition, nearbyCondition.Parameters)),
			nearbyCondition.Parameters);
		var savedPlace = await dbContext.Places.FirstOrDefaultAsync(nearbyAndNameCondition, cancellationToken);
		return savedPlace is null ? null : ToPlace(savedPlace);
	}

	private Expression<Func<Infrastructure.Models.Place, bool>> IsNearbyLocation(Location location1)
	{
		const double kilometersPerDegree = 111.1; // Approximate for both latitude and longitude
		const double latLongDifferenceEquivalentToM = 200 / kilometersPerDegree;
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