namespace Client.Services;

using CoreLocation;
using Models;

public partial class GeolocatorImplementation
{
	private readonly CLLocationManager manager = new();

	public void StartListening()
	{
		manager.LocationsUpdated += OnPositionChanged;
		manager.DistanceFilter = 100;
		manager.StartUpdatingLocation();
	}

	public void StopListening()
	{
		manager.LocationsUpdated -= OnPositionChanged;
		manager.StopUpdatingLocation();
	}

	void OnPositionChanged(object? sender, CLLocationsUpdatedEventArgs args)
	{
		if (args.Locations.Length > 0)
		{
			var lastLocation = args.Locations[^1];
			weakEventManager.HandleEvent(
				this,
				new GeolocatorData(new(lastLocation.Coordinate.Latitude, lastLocation.Coordinate.Longitude), lastLocation.Speed),
				nameof(PositionChanged));
		}
	}
}