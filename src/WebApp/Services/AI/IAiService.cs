namespace WebApp.Services.AI;

using Shared.Models;

public interface IAiService
{
	Task<List<Place>> GetNearByPlaces(Location location);

	Task<string?> GetPlaceDescription(string placeName, Location location);

	Task<string?> GenerateImage(string placeName, Location location);
}