namespace Client.Services;

using Models;
using Windows.Devices.Geolocation;

public partial class GeolocatorImplementation
{
	private readonly Geolocator locator = new();

	public void StartListening()
	{
		locator.PositionChanged += OnPositionChanged;

		locator.MovementThreshold = 100;
		locator.DesiredAccuracyInMeters = 100;
	}

	public void StopListening()
	{
		locator.PositionChanged -= OnPositionChanged;
	}

	void OnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
	{
		weakEventManager.HandleEvent(
			this,
			new GeolocatorData(new Location(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude), args.Position.Coordinate.Speed ?? 0),
			nameof(PositionChanged));
	}
}