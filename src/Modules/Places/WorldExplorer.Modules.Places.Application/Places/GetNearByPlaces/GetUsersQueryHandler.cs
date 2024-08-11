using System.Data.Common;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;

namespace WorldExplorer.Modules.Users.Application.Users.GetUser;

using Microsoft.Extensions.Logging;
using Places.Application.Places.GetPlace;
using Places.Domain.Places;
using Places.Infrastructure.Places;
using WebApp.Services.AI;

internal sealed class GetNearByPlacesQueryHandler(IPlaceRepository placeRepository, IAiService aiService, ILogger<GetNearByPlacesQuery> logger)
	: IQueryHandler<GetNearByPlacesQuery, List<PlaceResponse>>
{
	public async Task<Result<List<PlaceResponse>>> Handle(GetNearByPlacesQuery request, CancellationToken cancellationToken)
	{
		var p = await aiService.GetNearByPlaces(new Location(request.Latitude, request.Longitude));
		logger.LogInformation("GetNearByPlacesQueryHandler: {@Places}", p);
		//var locationRequestNearbyDistance = placesOptions.Value.LocationRequestNearbyDistance;
		//var userLocation = location.ToPoint();

		//var locationInfoRequests = await dbContext.LocationInfoRequests.Where(x => x.Location.IsWithinDistance(userLocation, locationRequestNearbyDistance)).ToListAsync(cancellationToken);

		//if (locationInfoRequests.Exists(x => x.Status != LocationInfoRequestStatus.Completed))
		//{
		//	return new OperationResult<List<Place>>
		//	{
		//		StatusCode = StatusCode.LocationInfoRequestPending
		//	};
		//}

		//var hasCompletedRequests = locationInfoRequests.Count > 0;
		//if (hasCompletedRequests)
		//{
		//	var nearbyDistance = placesOptions.Value.NearbyDistance;
		//	var nearestPlacesNearby = await dbContext.Places
		//											.Where(x => x.Location.IsWithinDistance(userLocation, nearbyDistance))
		//											.ToListAsync(cancellationToken);
		//	return new OperationResult<List<Place>>
		//	{
		//		Result = nearestPlacesNearby.Select(ToPlace).ToList()
		//	};
		//}

		//var locationInfoRequest = new LocationInfoRequest
		//{
		//	Status = LocationInfoRequestStatus.New,
		//	Location = userLocation,
		//	CreationDate = DateTime.UtcNow
		//};
		//await dbContext.LocationInfoRequests.AddAsync(locationInfoRequest, cancellationToken);
		//await dbContext.SaveChangesAsync(cancellationToken);
		//return new OperationResult<List<Place>>
		//{
		//	StatusCode = StatusCode.LocationInfoRequestPending
		//};

		return new List<PlaceResponse>();
	}
}
