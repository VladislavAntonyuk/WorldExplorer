namespace WebApp.Services.Place;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using WorldExplorer.Modules.Places.Application.Abstractions;
using WorldExplorer.Modules.Places.Domain.LocationInfo;
using WorldExplorer.Modules.Places.Domain.Places;
using WorldExplorer.Modules.Places.Infrastructure.Database;
using WorldExplorer.Modules.Places.Infrastructure.Image;

[DisallowConcurrentExecution]
internal sealed class PlaceDetailsJob(
	PlacesDbContext dbContext,
	IAiService aiService,
	IImageSearchService imageSearchService,
	ILogger<PlaceDetailsJob> logger) : IJob
{
	private const string ModuleName = "Places";
	private const int MinImagesCount = 70;

	public async Task Execute(IJobExecutionContext context)
	{
		logger.LogInformation("{Module} - Beginning to process locationInfoRequests messages", ModuleName);

		var notFilledPlaces = await dbContext.Places
											 .AsTracking()
											 .Where(x => x.Images.Count < MinImagesCount || string.IsNullOrEmpty(x.Description))
											 .OrderBy(x => x.Images.Count)
											 .ToListAsync(context.CancellationToken);

		if (notFilledPlaces.Count == 0)
		{
			return;
		}

		var placesChunks = notFilledPlaces.Chunk(10);
		foreach (var places in placesChunks)
		{
			var fillPlacesDescriptionsTasks = places.Select(GenerateDescription);
			var fillPlacesImagesTasks = places.Select(place => GenerateImages(place, context.CancellationToken));

			try
			{
				await Task.WhenAll(fillPlacesDescriptionsTasks);
				await Task.WhenAll(fillPlacesImagesTasks);
				await dbContext.SaveChangesAsync(context.CancellationToken);
			}
			catch (Exception e)
			{
				logger.LogError(e, "Error updating images for places");
			}
		}

		logger.LogInformation("{Module} - Completed processing inbox messages", ModuleName);
	}


	private async Task GenerateImages(Place place, CancellationToken stoppingToken)
	{
		if (place.Images.Count >= MinImagesCount)
		{
			return;
		}

		var images = new List<string>();
		images.AddRange(await imageSearchService.GetPlaceImages(place.Name, stoppingToken));
		foreach (var image in images)
		{
			place.Images.Add(new Image
			{
				Source = image
			});
		}
	}

	private async Task GenerateDescription(Place place)
	{
		if (string.IsNullOrWhiteSpace(place.Description))
		{
			var placeDescription = await aiService.GetPlaceDescription(place.Name, Location.FromPoint(place.Location));
			if (!string.IsNullOrWhiteSpace(placeDescription))
			{
				place.Update(place.Name, place.Location, placeDescription);
			}
		}
	}
}

