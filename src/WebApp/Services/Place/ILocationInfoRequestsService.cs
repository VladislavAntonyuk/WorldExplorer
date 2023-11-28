namespace WebApp.Services.Place;

using Infrastructure.Entities;

public interface ILocationInfoRequestsService
{
	Task Clear(CancellationToken cancellationToken);
	Task<List<LocationInfoRequest>> GetRequests(CancellationToken cancellationToken);
	Task Delete(int requestId, CancellationToken cancellationToken);
}