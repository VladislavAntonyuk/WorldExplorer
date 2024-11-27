namespace WorldExplorer.Modules.Places.Application.Places.GetPlaces;

using Common.Application.Messaging;
using GetPlace;

public sealed record GetPlacesQuery : IQuery<List<PlaceResponse>>;