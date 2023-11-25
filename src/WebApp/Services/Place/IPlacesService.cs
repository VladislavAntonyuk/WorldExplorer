namespace WebApp.Services.Place;

using Shared.Models;

public interface IPlacesService
{
	Task<List<Place>> GetPlaces(CancellationToken cancellationToken);
	Task<List<Place>> GetNearByPlaces(Location location, CancellationToken cancellationToken);
	Task<Place?> GetPlaceDetails(Guid id, CancellationToken cancellationToken);
	Task ClearPlaces(CancellationToken cancellationToken);
	bool IsNearby(Location location1, Location location2, double distance);
	Task Delete(Guid placeId, CancellationToken cancellationToken);
}