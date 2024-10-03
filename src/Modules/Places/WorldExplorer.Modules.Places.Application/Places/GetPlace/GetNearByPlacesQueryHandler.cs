namespace WorldExplorer.Modules.Places.Application.Places.GetPlace;

using Abstractions;
using Common.Application.Messaging;
using Common.Domain;
using Domain.Places;
using GetNearByPlaces;

internal sealed class GePlaceDetailsQueryHandler(IPlaceRepository placeRepository)
	: IQueryHandler<GetPlaceDetailsQuery, PlaceResponse>
{
	public async Task<Result<PlaceResponse>> Handle(GetPlaceDetailsQuery request, CancellationToken cancellationToken)
	{
		var place = await placeRepository.GetAsync(request.PlaceId, cancellationToken);
		if (place is null)
		{
			return Result.Failure<PlaceResponse>(PlaceErrors.NotFound(request.PlaceId));
		}

		return place.ToPlaceResponse();
	}
}