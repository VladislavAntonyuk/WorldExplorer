namespace WorldExplorer.Modules.Places.Application.Abstractions;

using NetTopologySuite.Geometries;

//latitude = y and longitude = x
public record Location(double Latitude, double Longitude)
{
	public static readonly Location Default = new(0, 0);

	public Point ToPoint()
	{
		return new Point(Longitude, Latitude)
		{
			SRID = DistanceConstants.SRID
		};
	}

	public static Location FromPoint(Point location)
	{
		return new Location(location.Y, location.X);
	}

	public double CalculateDistanceInMetersTo(Location placeLocation)
	{
		var point1 = placeLocation.ToPoint();
		var point2 = ToPoint();
		return Math.Round(point1.Distance(point2) * 100 * 1000, 1);
	}
}