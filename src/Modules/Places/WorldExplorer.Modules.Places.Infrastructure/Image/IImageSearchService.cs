namespace WorldExplorer.Modules.Places.Infrastructure.Image;

public interface IImageSearchService
{
	Task<List<string>> GetPlaceImages(string placeName, CancellationToken cancellationToken);
}