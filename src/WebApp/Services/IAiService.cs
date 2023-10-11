namespace WebApp.Services;

using Shared.Models;

public interface IAiService
{
	Task<List<Place>> GetNearByPlaces(Location location);
}