namespace WebApp.Services.Place;

using WorldExplorer.Modules.Places.Domain.Places;

public interface IPlacesService
{
	Task<List<Place>> GetPlaces(CancellationToken cancellationToken);
	//Task<OperationResult<List<Place>>> GetNearByPlaces(Location location, CancellationToken cancellationToken);
	Task<Place?> GetPlaceDetails(Guid id, CancellationToken cancellationToken);
	Task ClearPlaces(CancellationToken cancellationToken);
	bool IsNearby(Location location1, Location location2);
	Task Delete(Guid placeId, CancellationToken cancellationToken);
	Task UpdatePlace(Place place, CancellationToken cancellationToken);
}