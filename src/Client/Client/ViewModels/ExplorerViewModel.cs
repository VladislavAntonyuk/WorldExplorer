namespace Client.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Resources.Localization;
using Services;
using Services.API;
using Services.Navigation;
using Shared.Models;
using Views;
using WorldExplorer.Client.Map.WorldExplorerMap;
using Location = Location;

public sealed partial class ExplorerViewModel(IPlacesApi placesApi,
	ILauncher launcher,
	IGeolocation geoLocation,
	IDialogService dialogService,
	IDispatcher dispatcher,
	IDeviceDisplay deviceDisplay,
	INavigationService navigationService) : BaseViewModel, IDisposable
{
	[ObservableProperty]
	private Location? currentLocation;

	[ObservableProperty]
	private string? status;

	private void GeoLocationOnLocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
	{
		UpdateLocation(e.Location);
	}

	private void GeoLocationOnListeningFailed(object? sender, GeolocationListeningFailedEventArgs e)
	{
		Status = $"{Localization.UnableToGetPlaceDetails}. {e.Error}";
	}

	private void UpdateLocation(Location location)
	{
		CurrentLocation = location;
	}

	public ObservableCollection<WorldExplorerPin> Pins { get; } = [];
	
	public override async Task InitializeAsync()
	{
		deviceDisplay.KeepScreenOn = true;
		await base.InitializeAsync();
		await StartTracking();
	}

	public override Task UnInitializeAsync()
	{
		StopTracking();
		deviceDisplay.KeepScreenOn = false;
		return base.UnInitializeAsync();
	}

	[RelayCommand]
	private void ToggleUserLocation()
	{
		geoLocation.StopListeningForeground();
		CurrentLocation = null;
	}

	[RelayCommand]
	private Task Help()
	{
		return launcher.TryOpenAsync("https://world-explorer.azurewebsites.net/about");
	}

	[RelayCommand]
	private void About()
	{
		Application.Current?.OpenWindow(new Window(new AboutPage(new AboutViewModel())));
	}

	[RelayCommand(AllowConcurrentExecutions = false)]
	private async Task MarkerClicked(WorldExplorerPin? pin)
	{
		if (pin is null)
		{
			return;
		}

		await navigationService.NavigateAsync<PlaceDetailsViewModel, ErrorViewModel>(new Dictionary<string, object?>
		{
			{
				"place", pin.PlaceId
			}
		});
	}

	[RelayCommand(AllowConcurrentExecutions = false)]
	private async Task StartTracking()
	{
		var permission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
		if (permission != PermissionStatus.Granted)
		{
			await dialogService.AlertAsync("Error", "Geolocation permission denied", "OK");
			return;
		}

		var lastKnownLocation = await geoLocation.GetLastKnownLocationAsync();
		if (lastKnownLocation is not null)
		{
			UpdateLocation(lastKnownLocation);
		}

		Status = Localization.LookingForPlaces;
		geoLocation.LocationChanged += GeoLocationOnLocationChanged;
		geoLocation.ListeningFailed += GeoLocationOnListeningFailed;
		await geoLocation.StartListeningForegroundAsync(new GeolocationListeningRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(30)));
	}

	async partial void OnCurrentLocationChanged(Location? value)
	{
		if (value is null)
		{
			return;
		}

		int attempts = 0;
		StatusCode statusCode;
		do
		{
			statusCode = await dispatcher.DispatchAsync(() => GetRecommendations(value));
			attempts++;
			if (attempts > 5)
			{
				Status = Localization.UnableToGetPlaceDetails;
				break;
			}
		} while (statusCode == StatusCode.LocationInfoRequestPending);
	}

	private async Task<StatusCode> GetRecommendations(Location location)
	{
		var placesResponse = await placesApi.GetRecommendations(new Location(location.Latitude, location.Longitude), CancellationToken.None);

		if (!placesResponse.IsSuccessStatusCode)
		{
			await dialogService.ToastAsync(placesResponse.Error.Message);
			Status = Localization.UnableToGetPlaceDetails;
			return StatusCode.FailedResponse;
		}

		switch (placesResponse.Content.StatusCode)
		{
			case StatusCode.Success:
				if (placesResponse.Content.Result.Count == 0)
				{
					Status = Localization.NoPlacesFound;
					break;
				}

				Status = string.Format(Localization.FoundPlaces, placesResponse.Content.Result.Count);
				foreach (var place in placesResponse.Content.Result.Where(x => Pins.All(pin => pin.PlaceId != x.Id)))
				{
					Pins.Add(new WorldExplorerPin
					{
						PlaceId = place.Id,
						Location = place.Location,
						Label = place.Name,
						Image = place.MainImage,
						MarkerClicked = new AsyncRelayCommand<WorldExplorerPin>(MarkerClicked)
					});
				}

				await CheckLocation(location);

				break;
			case StatusCode.LocationInfoRequestPending:
				Status = Localization.LookingForPlaces;
				await Task.Delay(TimeSpan.FromSeconds(10));
				break;
			case StatusCode.FailedResponse:
				Status = Localization.UnableToGetPlaceDetails;
				break;
		}

		return placesResponse.Content.StatusCode;
	}

	private async Task CheckLocation(Location location)
	{
		WorldExplorerPin? closestPlace = null;
		double closestDistanceToPlace = 1;
		foreach (var pin in Pins)
		{
			var distanceToPlace = location.CalculateDistance(pin.Location, DistanceUnits.Kilometers);
			if (distanceToPlace < closestDistanceToPlace)
			{
				closestDistanceToPlace = distanceToPlace;
				closestPlace = pin;
			}
		}

		if (closestPlace is not null)
		{
			await dialogService.ToastAsync(string.Format(Localization.YouAreNear, closestPlace.Label),
										   CancellationToken.None);
		}
	}

	public void Dispose()
	{
		StopTracking();
	}

	private void StopTracking()
	{
		CurrentLocation = null;
		geoLocation.ListeningFailed -= GeoLocationOnListeningFailed;
		geoLocation.LocationChanged -= GeoLocationOnLocationChanged;
		geoLocation.StopListeningForeground();
	}
}