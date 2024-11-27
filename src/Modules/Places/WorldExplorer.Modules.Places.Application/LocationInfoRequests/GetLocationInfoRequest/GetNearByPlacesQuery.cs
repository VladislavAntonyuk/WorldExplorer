namespace WorldExplorer.Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequest;

using Common.Application.Messaging;
using GetLocationInfoRequests;

public sealed record GetLocationInfoRequestQuery(int Id) : IQuery<LocationInfoRequestResponse>;