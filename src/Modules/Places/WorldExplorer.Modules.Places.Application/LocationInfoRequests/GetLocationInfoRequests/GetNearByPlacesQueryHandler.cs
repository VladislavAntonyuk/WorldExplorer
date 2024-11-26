namespace WorldExplorer.Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequests;

using Abstractions;
using Domain.LocationInfo;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;

internal sealed class GePLocationInfoRequestsQueryHandler(ILocationInfoRepository placeRepository)
	: IQueryHandler<GetLocationInfoRequestsQuery, List<LocationInfoRequestResponse>>
{
	public async Task<Result<List<LocationInfoRequestResponse>>> Handle(GetLocationInfoRequestsQuery request, CancellationToken cancellationToken)
	{
		var places = await placeRepository.GetRequests(cancellationToken);
		return places.Select(ToPlace).ToList();
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