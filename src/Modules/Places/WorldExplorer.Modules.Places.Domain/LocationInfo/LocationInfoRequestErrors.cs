namespace WorldExplorer.Modules.Places.Domain.LocationInfo;

using Common.Domain;

public static class LocationInfoRequestErrors
{
	public static Error NotFound(int placeId)
	{
		return Error.NotFound("LocationInfoRequests.NotFound",
		                      $"The location info request with the identifier {placeId} not found");
	}
}