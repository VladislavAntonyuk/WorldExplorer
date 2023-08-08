namespace Shared.Models;

public record Location(double Latitude, double Longitude)
{
	public static readonly Location Default = new(0, 0);
}