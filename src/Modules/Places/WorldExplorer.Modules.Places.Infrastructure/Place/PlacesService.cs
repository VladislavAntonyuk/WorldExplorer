//namespace WebApp.Services.Place;

//using Infrastructure;
//using Infrastructure.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using Shared.Enums;
//using Shared.Models;
//using Location = Shared.Models.Location;
//using Place = Shared.Models.Place;
//using Review = Shared.Models.Review;

//public class PlacesService(IDbContextFactory<WorldExplorerDbContext> dbContextFactory, IOptions<PlacesSettings> placesOptions) : IPlacesService
//{
//	public async Task ClearPlaces(CancellationToken cancellationToken)
//	{
//		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
//		await dbContext.Places.ExecuteDeleteAsync(cancellationToken);
//	}

//	public async Task<List<Place>> GetPlaces(CancellationToken cancellationToken)
//	{
//		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
//		var dbPlaces = await dbContext.Places.ToListAsync(cancellationToken);
//		return [.. dbPlaces.Select(ToPlace).OrderBy(x => x.Name)];
//	}



//	public async Task<Place?> GetPlaceDetails(Guid id, CancellationToken cancellationToken)
//	{
//		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
//		var savedPlace = await dbContext.Places.FirstOrDefaultAsync(place => place.Id == id, cancellationToken);
//		return savedPlace is null ? null : ToPlace(savedPlace);
//	}

//	public bool IsNearby(Location location1, Location location2)
//	{
//		var point1 = location1.ToPoint();
//		var point2 = location2.ToPoint();
//		return point1.IsWithinDistance(point2, placesOptions.Value.LocationDistance);
//	}

//	public async Task Delete(Guid placeId, CancellationToken cancellationToken)
//	{
//		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
//		await dbContext.Places.Where(x => x.Id == placeId).ExecuteDeleteAsync(cancellationToken);
//	}

//	public async Task UpdatePlace(Place place, CancellationToken cancellationToken)
//	{
//		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
//		var dbPlace = await dbContext.Places.AsTracking().FirstOrDefaultAsync(x => x.Id == place.Id, cancellationToken);
//		if (dbPlace is not null)
//		{
//			ToPlace(dbPlace, place);
//			await dbContext.SaveChangesAsync(cancellationToken);
//		}
//	}

//	private static Place ToPlace(Infrastructure.Entities.Place place)
//	{
//		return new Place
//		{
//			Id = place.Id,
//			Name = place.Name,
//			Description = place.Description,
//			Images = place.Images.Select(x => x.Source).ToList(),
//			Location = place.Location.ToLocation(),
//			Reviews = place.Reviews.Select(x => new Review
//			{
//				Id = x.Id,
//				Comment = x.Comment,
//				Rating = x.Rating,
//				ReviewDate = x.ReviewDate
//			}).ToList()
//		};
//	}

//	private static void ToPlace(Infrastructure.Entities.Place dbPlace, Place place)
//	{
//		dbPlace.Id = place.Id;
//		dbPlace.Name = place.Name;
//		dbPlace.Description = place.Description;
//		dbPlace.Images = place.Images.Select(x => new Image
//		{
//			Source = x
//		}).ToList();
//		dbPlace.Location = place.Location.ToPoint();
//	}
//}