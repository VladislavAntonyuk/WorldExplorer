namespace Client;

using Models;
using Services;
using Tizen.Location;

public partial class GeolocatorImplementation
{
	private readonly Locator locator = new(LocationType.Gps)
	{
		Distance = 100
	};

	public void StartListening()
	{
		locator.DistanceBasedLocationChanged += OnPositionChanged;
		locator.Start();
	}

	public void StopListening()
	{
		locator.DistanceBasedLocationChanged -= OnPositionChanged;
		locator.Stop();
	}

	void OnPositionChanged(object? sender, LocationChangedEventArgs args)
	{
		if (args.Locations.Length > 0)
		{
			var lastLocation = args.Locations[^1];
			weakEventManager.HandleEvent(
				this,
				new GeolocatorData(new(args.Location.Latitude, args.Location.Longitude), args.Location.Speed),
				nameof(PositionChanged));
		}
	}
}