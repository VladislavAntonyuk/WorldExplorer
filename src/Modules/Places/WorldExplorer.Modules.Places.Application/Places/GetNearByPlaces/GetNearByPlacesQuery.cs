namespace WorldExplorer.Modules.Places.Application.Places.GetNearByPlaces;

using Common.Application.Messaging;
using Domain.LocationInfo;
using GetPlace;
using NetTopologySuite.Geometries;

public sealed record GetNearByPlacesQuery(double Longitude, double Latitude)
	: IQuery<OperationResult<List<PlaceResponse>>>
{
	public Point ToPoint()
	{
		return new Point(Latitude, Longitude)
		{
			SRID = 4326
		};
	}
}