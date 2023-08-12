namespace Client;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using Models;
using Services;

public class GeolocatorImplementation : IGeolocator
{
	private GeolocationContinuousListener? locator;

	public async Task StartListening(IProgress<GeolocatorData> positionChangedProgress,
		CancellationToken cancellationToken)
	{
		locator = new GeolocationContinuousListener();
		var taskCompletionSource = new TaskCompletionSource();
		cancellationToken.Register(() =>
		{
			locator.Dispose();
			locator = null;
			taskCompletionSource.TrySetResult();
		});
		locator.OnLocationChangedAction = location =>
			positionChangedProgress.Report(
				new GeolocatorData(new(location.Latitude, location.Longitude), location.Speed));
		await taskCompletionSource.Task;
	}
}

internal class GeolocationContinuousListener : Object, ILocationListener
{
	private readonly LocationManager? locationManager;

	public GeolocationContinuousListener()
	{
		locationManager = (LocationManager?)Application.Context.GetSystemService(Context.LocationService);
		locationManager?.RequestLocationUpdates(LocationManager.GpsProvider, 1000, 100, this);
	}

	public Action<Location>? OnLocationChangedAction { get; set; }

	public void OnProviderDisabled(string provider)
	{
	}

	public void OnProviderEnabled(string provider)
	{
	}

	public void OnStatusChanged(string? provider, [GeneratedEnum] Availability status, Bundle? extras)
	{
	}

	public void OnLocationChanged(Location location)
	{
		OnLocationChangedAction?.Invoke(location);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		locationManager?.RemoveUpdates(this);
		locationManager?.Dispose();
	}
}