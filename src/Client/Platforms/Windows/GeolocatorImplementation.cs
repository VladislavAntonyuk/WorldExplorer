namespace Client;

using Services;
using Windows.Devices.Geolocation;
using Geolocator = Windows.Devices.Geolocation.Geolocator;

public class GeolocatorImplementation : IGeolocator
{
	private readonly Geolocator locator = new();

	public async Task StartListening(IProgress<Location> positionChangedProgress, CancellationToken cancellationToken)
	{
		var taskCompletionSource = new TaskCompletionSource();
		cancellationToken.Register(() =>
		{
			locator.PositionChanged -= PositionChanged;
			taskCompletionSource.TrySetResult();
		});
		locator.PositionChanged += PositionChanged;

		void PositionChanged(Geolocator sender, PositionChangedEventArgs args)
		{
			positionChangedProgress.Report(new Location(args.Position.Coordinate.Latitude,
														args.Position.Coordinate.Longitude));
		}

		locator.MovementThreshold = 100;

		await taskCompletionSource.Task;
	}
}