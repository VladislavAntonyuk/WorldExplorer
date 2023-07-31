namespace Client;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using Services;
using Location = Microsoft.Maui.Devices.Sensors.Location;

public class GeolocatorImplementation : IGeolocator
{
	private readonly IDialogService dialogService;
	private GeolocationContinuousListener? locator;

	public GeolocatorImplementation(IDialogService dialogService)
	{
		this.dialogService = dialogService;
	}

	public async Task StartListening(IProgress<Location> positionChangedProgress, CancellationToken cancellationToken)
	{
		var permission = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
		if (permission != PermissionStatus.Granted)
		{
			permission = await Permissions.RequestAsync<Permissions.LocationAlways>();
			if (permission != PermissionStatus.Granted)
			{
				await dialogService.ToastAsync("No permission", CancellationToken.None);
				return;
			}
		}

		locator = new GeolocationContinuousListener();
		var taskCompletionSource = new TaskCompletionSource();
		cancellationToken.Register(() =>
		{
			locator.Dispose();
			locator = null;
			taskCompletionSource.TrySetResult();
		});
		locator.OnLocationChangedAction = location =>
			positionChangedProgress.Report(new Location(location.Latitude, location.Longitude));
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

	public Action<Android.Locations.Location>? OnLocationChangedAction { get; set; }

	public void OnLocationChanged(Android.Locations.Location location)
	{
		OnLocationChangedAction?.Invoke(location);
	}

	public void OnProviderDisabled(string provider)
	{
	}

	public void OnProviderEnabled(string provider)
	{
	}

	public void OnStatusChanged(string? provider, [GeneratedEnum] Availability status, Bundle? extras)
	{
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		locationManager?.RemoveUpdates(this);
		locationManager?.Dispose();
	}
}