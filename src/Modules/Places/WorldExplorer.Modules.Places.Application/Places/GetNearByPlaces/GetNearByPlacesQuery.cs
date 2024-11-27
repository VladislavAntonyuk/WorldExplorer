namespace WorldExplorer.Modules.Places.Application.Places.GetNearByPlaces;

using Abstractions;
using Common.Application.Messaging;
using Domain.LocationInfo;
using GetPlace;

public sealed record GetNearByPlacesQuery(Location Location) : IQuery<OperationResult<List<PlaceResponse>>>;