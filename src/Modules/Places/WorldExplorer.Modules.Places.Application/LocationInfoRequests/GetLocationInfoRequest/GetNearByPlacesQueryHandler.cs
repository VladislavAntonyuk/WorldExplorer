namespace WorldExplorer.Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequest;

using Abstractions;
using Domain.LocationInfo;
using GetLocationInfoRequests;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;

internal sealed class GePLocationInfoRequestsQueryHandler(ILocationInfoRepository placeRepository)
	: IQueryHandler<GetLocationInfoRequestQuery, LocationInfoRequestResponse>
{
	public async Task<Result<LocationInfoRequestResponse>> Handle(GetLocationInfoRequestQuery request, CancellationToken cancellationToken)
	{
		var places = await placeRepository.GetAsync(request.Id, cancellationToken);
		if (places is null)
		{
			return Result.Failure<LocationInfoRequestResponse>(LocationInfoRequestErrors.NotFound(request.Id));
		}

		return ToPlace(places);
	}

	private LocationInfoRequestResponse ToPlace(LocationInfoRequest place)
	{
		return new LocationInfoRequestResponse
		{
			Location = new Location(place.Location.Y, place.Location.X),
			CreationDate = place.CreationDate,
			Id = place.Id,
			Status = place.Status
		};
	}
}