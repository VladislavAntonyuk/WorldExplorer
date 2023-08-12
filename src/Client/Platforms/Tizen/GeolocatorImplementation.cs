namespace Client;

using Models;
using Services;
using Tizen.Location;

public class GeolocatorImplementation : IGeolocator
{
	private readonly Locator locator = new(LocationType.Gps);

	public async Task StartListening(IProgress<GeolocatorData> positionChangedProgress, CancellationToken cancellationToken)
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
			positionChangedProgress.Report(new GeolocatorData(new (args.Location.Latitude, args.Location.Longitude), args.Location.Speed));
		}

		locator.Distance = 100;
		locator.Start();
		await taskCompletionSource.Task;
	}
}