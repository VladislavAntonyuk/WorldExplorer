namespace WorldExplorer.Modules.Places.Application.Abstractions;

using Domain.Places;

public interface IAiService
{
	Task<List<Place>> GetNearByPlaces(Location location);

	Task<string?> GetPlaceDescription(string placeName, Location location);
}