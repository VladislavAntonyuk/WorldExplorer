namespace WebApp.Services.AI;

using WorldExplorer.Modules.Places.Domain.Places;

public interface IAiService
{
	Task<List<Place>> GetNearByPlaces(Location location);

	Task<string?> GetPlaceDescription(string placeName, Location location);

	Task<string?> GenerateImage(string placeName, Location location);
}