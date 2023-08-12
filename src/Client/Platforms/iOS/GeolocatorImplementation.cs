namespace Client;

using CoreLocation;
using Models;
using Services;

public class GeolocatorImplementation : IGeolocator
{
	private readonly CLLocationManager manager = new();

	public async Task StartListening(IProgress<GeolocatorData> positionChangedProgress,
		CancellationToken cancellationToken)
	{
		var taskCompletionSource = new TaskCompletionSource();
		cancellationToken.Register(() =>
		{
			manager.LocationsUpdated -= PositionChanged;
			manager.StopUpdatingLocation();
			taskCompletionSource.TrySetResult();
		});
		manager.LocationsUpdated += PositionChanged;
		manager.DistanceFilter = 100;
		manager.StartUpdatingLocation();

		void PositionChanged(object? sender, CLLocationsUpdatedEventArgs args)
		{
			if (args.Locations.Length > 0)
			{
				var lastLocation = args.Locations[^1];
				positionChangedProgress.Report(new GeolocatorData(
												   new(lastLocation.Coordinate.Latitude,
													   lastLocation.Coordinate.Longitude), lastLocation.Speed));
			}
		}

		await taskCompletionSource.Task;
	}
}