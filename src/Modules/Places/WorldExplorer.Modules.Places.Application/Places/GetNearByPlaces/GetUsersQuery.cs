using WorldExplorer.Common.Application.Messaging;

namespace WorldExplorer.Modules.Users.Application.Users.GetUser;

using Places.Application.Places.GetPlace;

public sealed record GetNearByPlacesQuery(double Longitude, double Latitude) : IQuery<List<PlaceResponse>>;
