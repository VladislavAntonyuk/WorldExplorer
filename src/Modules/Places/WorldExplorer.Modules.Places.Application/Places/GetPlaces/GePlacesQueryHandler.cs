namespace WorldExplorer.Modules.Places.Application.Places.GetPlaces;

using Common.Application.Messaging;
using Common.Domain;
using Domain.Places;
using GetNearByPlaces;
using GetPlace;

internal sealed class GePlacesQueryHandler(IPlaceRepository placeRepository)
	: IQueryHandler<GetPlacesQuery, List<PlaceResponse>>
{
	public async Task<Result<List<PlaceResponse>>> Handle(GetPlacesQuery request, CancellationToken cancellationToken)
	{
		var places = await placeRepository.GetAsync(cancellationToken);
		return places.Select(x => x.ToPlaceResponse()).ToList();
	}
}