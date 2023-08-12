namespace Client;

using Models;
using Services;
using Windows.Devices.Geolocation;

public class GeolocatorImplementation : IGeolocator
{
	private readonly Geolocator locator = new();

	public async Task StartListening(IProgress<GeolocatorData> positionChangedProgress,
		CancellationToken cancellationToken)
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
			positionChangedProgress.Report(new GeolocatorData(
											   new Location(args.Position.Coordinate.Latitude,
															args.Position.Coordinate.Longitude),
											   args.Position.Coordinate.Speed ?? 0));
		}

		locator.MovementThreshold = 100;
		locator.DesiredAccuracyInMeters = 100;

		await taskCompletionSource.Task;
	}
}