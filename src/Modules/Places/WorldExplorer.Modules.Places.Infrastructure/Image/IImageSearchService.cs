namespace WorldExplorer.Modules.Places.Infrastructure.Image;

using Application.Abstractions;

public interface IImageSearchService
{
	Task<List<string>> GetPlaceImages(string placeName, CancellationToken cancellationToken);

	Task<string?> GenerateImage(string placeName, Location location, CancellationToken cancellationToken);
}