namespace WorldExplorer.Modules.Places.Application.Places.GetPlaces;

using GetNearByPlaces;
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
		return places.Select(x=>x.ToPlaceResponse()).ToList();
	}
}