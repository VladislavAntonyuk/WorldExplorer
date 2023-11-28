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
}