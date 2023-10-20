namespace WebApp.Services.AI;

using Shared.Models;

public interface IAiService
{
	Task<List<Place>> GetNearByPlaces(Location location);
}