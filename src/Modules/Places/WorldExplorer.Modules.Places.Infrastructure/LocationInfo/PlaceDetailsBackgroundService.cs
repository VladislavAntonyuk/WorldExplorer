//namespace WebApp.Services.Place;

//using AI;
//using Image;
//using Infrastructure;
//using Infrastructure.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Place = Microsoft.EntityFrameworkCore.DbLoggerCategory.Infrastructure.Entities.Place;

//public class PlaceDetailsBackgroundService(IDbContextFactory<WorldExplorerDbContext> dbContextFactory,
//	IAiService aiService,
//	IImageSearchService imageSearchService,
//	ILogger<PlaceDetailsBackgroundService> logger) : BackgroundService
//{
//	private const int MinImagesCount = 70;

//	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//	{
//		while (!stoppingToken.IsCancellationRequested)
//		{
//			await using var dbContext = await dbContextFactory.CreateDbContextAsync(stoppingToken);
//			var notFilledPlaces = await dbContext.Places
//													  .AsTracking()
//													  .Where(x => x.Images.Count < MinImagesCount)
//													  .OrderBy(x => x.Images.Count)
//													  .ToListAsync(stoppingToken);

//			if (notFilledPlaces.Count == 0)
//			{
//				await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
//			}

//			var placesChunks = notFilledPlaces.Chunk(10);
//			foreach (var places in placesChunks)
//			{
//				var fillPlacesTasks = places.Select(async place =>
//				{
//					await GenerateDescription(place);
//					await GenerateImages(place, stoppingToken);
//				});

//				try
//				{
//					await Task.WhenAll(fillPlacesTasks);
//					dbContext.Places.UpdateRange(places.Where(x => x.Images.Count > 0));
//					await dbContext.SaveChangesAsync(stoppingToken);
//				}
//				catch (Exception e)
//				{
//					logger.LogError(e, "Error updating images for places");
//				}
//			}
//		}
//	}

//	private async Task GenerateImages(Place place, CancellationToken stoppingToken)
//	{
//		var images = new List<string>();
//		var mainImage = await aiService.GenerateImage(place.Name, place.Location.ToLocation());
//		if (!string.IsNullOrWhiteSpace(mainImage))
//		{
//			images.Add(mainImage);
//		}

//		images.AddRange(await imageSearchService.GetPlaceImages(place.Name, stoppingToken));
//		foreach (var image in images)
//		{
//			place.Images.Add(new Image
//			{
//				Source = image
//			});
//		}
//	}

//	private async Task GenerateDescription(Place place)
//	{
//		if (string.IsNullOrWhiteSpace(place.Description))
//		{
//			place.Description = await aiService.GetPlaceDescription(place.Name, place.Location.ToLocation());
//		}
//	}
//}

