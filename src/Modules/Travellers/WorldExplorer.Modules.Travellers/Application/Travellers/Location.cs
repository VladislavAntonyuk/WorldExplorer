namespace WorldExplorer.Modules.Travellers.Application.Travellers;
public record Location(double Latitude, double Longitude)
{
	public static readonly Location Default = new(0, 0);
}