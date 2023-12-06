namespace WebApp.Infrastructure;

using NetTopologySuite.Geometries;
using Services.Place;

public static class LocationExtensions
{
	public static Point ToPoint(this Shared.Models.Location location)
	{
		return new Point(location.Longitude, location.Latitude)
		{
			SRID = DistanceConstants.SRID
		};
	}

	public static Shared.Models.Location ToLocation(this Point point)
	{
		return new Shared.Models.Location(point.Y, point.X);
	}
}