namespace WorldExplorer.Modules.Places.Application.Abstractions;

using Domain.Places;

public interface IAiService
{
	Task<List<Place>> GetNearByPlaces(Location location, CancellationToken cancellationToken);

	Task<string?> GetPlaceDescription(string placeName, Location location, CancellationToken cancellationToken);
}