namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Domain.Places;
using NetTopologySuite.Geometries;

public interface IPlaceRepository
{
	Task<Place?> GetAsync(Guid id, CancellationToken cancellationToken = default);
	void Insert(Place place);
	Task<List<Place>> GetNearestPlacesAsync(Point userLocation, CancellationToken cancellationToken);
}