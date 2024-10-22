namespace WorldExplorer.Modules.Places.Application.Places.GetPlace;

using Common.Application.Messaging;

public sealed record GetPlaceDetailsQuery(Guid PlaceId) : IQuery<PlaceResponse>;