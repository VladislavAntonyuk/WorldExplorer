namespace WorldExplorer.Modules.Places.Domain.Places;

using NetTopologySuite.Geometries;

public interface IPlaceRepository
{
	Task<Place?> GetAsync(Guid id, CancellationToken cancellationToken = default);
	Task<List<Place>> GetAsync(CancellationToken cancellationToken = default);
	void Insert(Place place);
	Task<List<Place>> GetNearestPlacesAsync(Point userLocation, CancellationToken cancellationToken);
}