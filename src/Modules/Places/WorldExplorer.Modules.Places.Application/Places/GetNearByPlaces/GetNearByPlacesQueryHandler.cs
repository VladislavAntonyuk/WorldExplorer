namespace WorldExplorer.Modules.Places.Application.Places.GetNearByPlaces;

using Abstractions.Data;
using Common.Application.Messaging;
using Common.Domain;
using Domain.LocationInfo;
using Domain.Places;
using GetPlace;

internal sealed class GetNearByPlacesQueryHandler(
	ILocationInfoRepository locationInfoRepository,
	IPlaceRepository placeRepository,
	IUnitOfWork unitOfWork) : IQueryHandler<GetNearByPlacesQuery, OperationResult<List<PlaceResponse>>>
{
	public async Task<Result<OperationResult<List<PlaceResponse>>>> Handle(GetNearByPlacesQuery request,
		CancellationToken cancellationToken)
	{
		var userLocation = request.Location.ToPoint();

		var locationInfoRequests = await locationInfoRepository.IsNearby(userLocation);

		if (locationInfoRequests.Exists(x => x.Status != LocationInfoRequestStatus.Completed))
		{
			return new OperationResult<List<PlaceResponse>>
			{
				StatusCode = StatusCode.LocationInfoRequestPending
			};
		}

		var hasCompletedRequests = locationInfoRequests.Count > 0;
		if (hasCompletedRequests)
		{
			var nearestPlacesNearby = await placeRepository.GetNearestPlacesAsync(userLocation, cancellationToken);
			return new OperationResult<List<PlaceResponse>>
			{
				Result = nearestPlacesNearby.Select(x => x.ToPlaceResponse()).ToList()
			};
		}

		var locationInfoRequest = new LocationInfoRequest
		{
			Status = LocationInfoRequestStatus.New,
			Location = userLocation,
			CreationDate = DateTime.UtcNow
		};
		locationInfoRepository.Insert(locationInfoRequest);
		await unitOfWork.SaveChangesAsync(cancellationToken);
		return new OperationResult<List<PlaceResponse>>
		{
			StatusCode = StatusCode.LocationInfoRequestPending
		};
	}
}