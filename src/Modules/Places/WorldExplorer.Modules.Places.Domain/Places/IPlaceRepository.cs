namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Domain.Places;

public interface IPlaceRepository
{
	Task<Place?> GetAsync(Guid id, CancellationToken cancellationToken = default);
	void Insert(Place place);
	Task<List<Place>> GetNearestPlacesAsync(Location userLocation, CancellationToken cancellationToken);
}