namespace WorldExplorer.Modules.Places.Domain.LocationInfo;

using NetTopologySuite.Geometries;

public interface ILocationInfoRepository
{
	Task<LocationInfoRequest?> GetAsync(int id, CancellationToken cancellationToken = default);
	void Insert(LocationInfoRequest locationInfoRequest);
	Task<List<LocationInfoRequest>> IsNearby(Point userLocation);
	Task Clear(CancellationToken cancellationToken);
	Task<List<LocationInfoRequest>> GetRequests(CancellationToken cancellationToken);
	Task Delete(int requestId, CancellationToken cancellationToken);
}