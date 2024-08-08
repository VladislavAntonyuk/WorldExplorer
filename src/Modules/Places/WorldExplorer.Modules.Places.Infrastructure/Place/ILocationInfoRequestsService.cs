namespace WebApp.Services.Place;

public interface ILocationInfoRequestsService
{
	Task Clear(CancellationToken cancellationToken);
	//Task<List<LocationInfoRequest>> GetRequests(CancellationToken cancellationToken);
	Task Delete(int requestId, CancellationToken cancellationToken);
}