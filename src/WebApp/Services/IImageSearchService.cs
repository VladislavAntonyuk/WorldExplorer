namespace WebApp.Services;

public interface IImageSearchService
{
	Task<List<string>> GetPlaceImages(string placeName, CancellationToken cancellationToken);
}