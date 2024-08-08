namespace WebApp.Services.Image;

public interface IImageSearchService
{
	Task<List<string>> GetPlaceImages(string placeName, CancellationToken cancellationToken);
}