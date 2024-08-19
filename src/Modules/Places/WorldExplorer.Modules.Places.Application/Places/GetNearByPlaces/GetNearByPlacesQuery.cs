using WorldExplorer.Common.Application.Messaging;

namespace WorldExplorer.Modules.Users.Application.Users.GetUser;

using Places.Application.Places.GetPlace;
using Places.Domain.Places;
using Shared.Models;

public sealed record GetNearByPlacesQuery(double Longitude, double Latitude) : IQuery<OperationResult<List<PlaceResponse>>>
{
	public Location ToPoint()
	{
		return new Location(Latitude, Longitude);
	}
}
