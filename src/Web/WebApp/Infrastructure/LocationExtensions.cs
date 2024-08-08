namespace WebApp.Infrastructure;

using NetTopologySuite.Geometries;
using Services.Place;
using Location = Shared.Models.Location;

public static class LocationExtensions
{
	public static Point ToPoint(this Location location)
	{
		return new Point(location.Longitude, location.Latitude)
		{
			SRID = DistanceConstants.SRID
		};
	}

	public static Location ToLocation(this Point point)
	{
		return new Location(point.Y, point.X);
	}
}