namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Application.Abstractions;
using Application.Abstractions.Data;
using Database;
using Domain.Places;
using Image;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

[DisallowConcurrentExecution]
internal sealed class PlaceDetailsJob(
	PlacesDbContext dbContext,
	IUnitOfWork unitOfWork,
	IAiService aiService,
	IImageSearchService imageSearchService,
	IOptions<PlacesJobOptions> placeJobOptions,
	ILogger<PlaceDetailsJob> logger) : IJob
{
	private const string ModuleName = "Places";

	public async Task Execute(IJobExecutionContext context)
	{
		logger.LogInformation("{Module} - Beginning to process locationInfoRequests messages", ModuleName);

		var notFilledPlaces = await dbContext.Places
											 .AsTracking()
											 .Include(x => x.Images)
											 .Where(x => x.Images.Count == 0 || string.IsNullOrEmpty(x.Description))
											 .OrderBy(x => x.CreatedAt)
											 .Take(placeJobOptions.Value.BatchSize)
											 .ToListAsync(context.CancellationToken);

		if (notFilledPlaces.Count == 0)
		{
			return;
		}

		var fillPlaceDetails = notFilledPlaces.Select(place => GeneratePlaceDetails(place, context.CancellationToken));
		await foreach (var task in Task.WhenEach(fillPlaceDetails))
		{
			var placeDetails = await task;
			try
			{
				placeDetails.Place.Update(placeDetails.Place.Name, placeDetails.Place.Location, placeDetails.Description, placeDetails.Images);
				await unitOfWork.SaveChangesAsync(context.CancellationToken);
			}
			catch (Exception e)
			{
				logger.LogError(e, "Error updating images for place {PlaceId}", placeDetails.Place.Id);
			}
		}

		logger.LogInformation("{Module} - Completed processing inbox messages", ModuleName);
	}

	private async Task<PlaceDetails> GeneratePlaceDetails(Place place, CancellationToken cancellationToken)
	{
		var placeDescription = place.Description;
		var images = place.Images.ToList();
		if (images.Count == 0)
		{
			var generatedImage = await imageSearchService.GenerateImage(place.Name, Location.FromPoint(place.Location), cancellationToken);
			if (!string.IsNullOrWhiteSpace(generatedImage))
			{
				images.Add(new PlaceImage
				{
					Source = $"data:image;base64,{generatedImage}"
				});
			}

			var placeImages = await imageSearchService.GetPlaceImages(place.Name, cancellationToken);
			images.AddRange(placeImages.Select(image => new PlaceImage
			{
				Source = image
			}));
		}

		if (string.IsNullOrWhiteSpace(placeDescription))
		{
			placeDescription = await aiService.GetPlaceDescription(place.Name, Location.FromPoint(place.Location), cancellationToken);
		}

		return new PlaceDetails(place, placeDescription, images);
	}

	private sealed record PlaceDetails(Place Place, string? Description, ICollection<PlaceImage> Images);
}