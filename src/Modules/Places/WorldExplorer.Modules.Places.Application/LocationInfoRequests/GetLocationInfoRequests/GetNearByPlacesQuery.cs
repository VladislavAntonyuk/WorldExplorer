namespace WorldExplorer.Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequests;

using WorldExplorer.Common.Application.Messaging;

public sealed record GetLocationInfoRequestsQuery : IQuery<List<LocationInfoRequestResponse>>;