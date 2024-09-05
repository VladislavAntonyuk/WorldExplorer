namespace WorldExplorer.Modules.Places.Application.Places.GetPlaces;

using Abstractions;
using GetPlace;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Places.Domain.Places;

internal sealed class GePlacesQueryHandler(IPlaceRepository placeRepository)
	: IQueryHandler<GetPlacesQuery, List<PlaceResponse>>
{
	public async Task<Result<List<PlaceResponse>>> Handle(GetPlacesQuery request, CancellationToken cancellationToken)
	{
		var places = await placeRepository.GetAsync(cancellationToken);
		return places.Select(ToPlace).ToList();
	}

	private PlaceResponse ToPlace(Place place)
	{
		return new PlaceResponse(place.Id, place.Name, place.Description,
		                         new Location(place.Location.X, place.Location.Y), 1,
		                         place.Images.Select(x => x.Source).ToList());
	}
}