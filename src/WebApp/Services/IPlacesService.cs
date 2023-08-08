namespace WebApp.Services;

using global::Shared.Models;

public interface IPlacesService
{
	Task<List<Place>> GetPlaces(CancellationToken cancellationToken);
	Task<List<Place>> GetNearByPlaces(Location location, CancellationToken cancellationToken);
	Task<Place?> GetPlaceDetails(string name, Location location, CancellationToken cancellationToken);
	Task ClearPlaces(CancellationToken cancellationToken);
	bool IsNearby(Location location1, Location location2, double distance);
}