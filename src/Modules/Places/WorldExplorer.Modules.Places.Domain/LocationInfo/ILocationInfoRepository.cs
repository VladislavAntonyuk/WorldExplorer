namespace WorldExplorer.Modules.Places.Domain.LocationInfo;

using Domain.Places;

public interface ILocationInfoRepository
{
	Task<LocationInfoRequest?> GetAsync(Guid id, CancellationToken cancellationToken = default);
	void Insert(LocationInfoRequest locationInfoRequest);
	Task<List<LocationInfoRequest>> IsNearby(Location userLocation);
	Task Clear(CancellationToken cancellationToken);
	Task<List<LocationInfoRequest>> GetRequests(CancellationToken cancellationToken);
	Task Delete(int requestId, CancellationToken cancellationToken);
}