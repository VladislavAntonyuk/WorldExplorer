namespace WorldExplorer.Modules.Places.Application.Places.GetNearByPlaces;

using Domain.LocationInfo;
using GetPlace;
using NetTopologySuite.Geometries;
using WorldExplorer.Common.Application.Messaging;

public sealed record GetNearByPlacesQuery(double Longitude, double Latitude) : IQuery<OperationResult<List<PlaceResponse>>>
{
	public Point ToPoint()
	{
		return new Point(Latitude, Longitude)
		{
			SRID = 4326
		};
	}
}
