namespace WorldExplorer.Modules.Places.Domain.Places;

using Common.Domain;

public static class PlaceErrors
{
	public static Error NotFound(Guid placeId) =>
		Error.NotFound("Places.NotFound", $"The place with the identifier {placeId} not found");
}