namespace WorldExplorer.Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequests;

using Common.Application.Messaging;

public sealed record GetLocationInfoRequestsQuery : IQuery<List<LocationInfoRequestResponse>>;