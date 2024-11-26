namespace WorldExplorer.Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequest;

using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequests;

public sealed record GetLocationInfoRequestQuery(int Id) : IQuery<LocationInfoRequestResponse>;