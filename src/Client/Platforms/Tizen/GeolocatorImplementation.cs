namespace Client;

using Services;
using Tizen.Location;
using Location = Microsoft.Maui.Devices.Sensors.Location;

public class GeolocatorImplementation : IGeolocator
{
	private readonly Locator locator = new(LocationType.Gps);

	public async Task StartListening(IProgress<Location> positionChangedProgress, CancellationToken cancellationToken)
	{
		var taskCompletionSource = new TaskCompletionSource();
		cancellationToken.Register(() =>
		{
			locator.DistanceBasedLocationChanged -= PositionChanged;
			locator.Stop();
			taskCompletionSource.TrySetResult();
		});
		locator.DistanceBasedLocationChanged += PositionChanged;

		void PositionChanged(object? sender, LocationChangedEventArgs args)
		{
			positionChangedProgress.Report(new Location(args.Location.Latitude, args.Location.Longitude));
		}

		locator.Distance = 100;
		locator.Start();
		await taskCompletionSource.Task;
	}
}