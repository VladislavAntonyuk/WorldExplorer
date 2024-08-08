namespace WebApp.Services.Place;

public class PlacesSettings
{
	public double LocationRequestNearbyDistance { get; set; } = 2000d / 100 / 1000; // 2000 meters
	public double NearbyDistance { get; set; } = 10000d / 100 / 1000; // 10000 meters
	public double LocationDistance { get; set; } = 100d / 100 / 1000; // 100 meters
}