namespace WorldExplorer.Modules.Places.Application.Places.GetNearByPlaces;

using Abstractions;
using Domain.Places;
using GetPlace;

internal static class PlaceExtensions
{
	public static PlaceResponse ToPlaceResponse(this Place place)
	{
		return new PlaceResponse(place.Id, place.Name, place.Description, Location.FromPoint(place.Location),
		                         place.Images.Select(x => x.Source).ToList());
	}
}