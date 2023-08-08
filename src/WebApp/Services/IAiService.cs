namespace WebApp.Services;

using global::Shared.Models;

public interface IAiService
{
	Task<List<Place>> GetNearByPlaces(Location location);
}