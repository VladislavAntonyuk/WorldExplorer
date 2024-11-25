namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Application.Abstractions;
using Database;
using Domain.Places;
using Image;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

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
											 .Include(x => x.Images)
											 .Where(x => x.Images.Count < MinImagesCount || string.IsNullOrEmpty(x.Description))
											 .OrderBy(x => x.Images.Count)
											 .ToListAsync(context.CancellationToken);

		if (notFilledPlaces.Count == 0)
		{
			return;
		}

		var fillPlaceDetails = notFilledPlaces.Select(place => GeneratePlaceDetails(place, context.CancellationToken));
		try
		{
			await foreach (var _ in Task.WhenEach(fillPlaceDetails))
			{
				await dbContext.SaveChangesAsync(context.CancellationToken);
			}
		}
		catch (Exception e)
		{
			logger.LogError(e, "Error updating images for places");
		}

		logger.LogInformation("{Module} - Completed processing inbox messages", ModuleName);
	}

	private async Task GeneratePlaceDetails(Place place, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(place.Description))
		{
			var placeDescription = await aiService.GetPlaceDescription(place.Name, Location.FromPoint(place.Location));
			if (!string.IsNullOrWhiteSpace(placeDescription))
			{
				place.Update(place.Name, place.Location, placeDescription);
			}

			if (place.Images.Count < MinImagesCount)
			{
				var image = await imageSearchService.GenerateImage(place.Name, Location.FromPoint(place.Location), cancellationToken);
				if (!string.IsNullOrWhiteSpace(image))
				{
					place.Images.Add(new PlaceImage
					{
						Source = $"data:image;base64,{image}"
					});
				}
			}
		}

		if (place.Images.Count >= MinImagesCount)
		{
			return;
		}

		var images = new List<string>();
		images.AddRange(await imageSearchService.GetPlaceImages(place.Name, cancellationToken));
		foreach (var image in images)
		{
			place.Images.Add(new PlaceImage
			{
				Source = image
			});
		}
	}
}

