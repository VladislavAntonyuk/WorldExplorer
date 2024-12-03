namespace Client.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.WorldExplorerMap;
using Framework;
using MvvmHelpers;
using Resources.Localization;
using Services;
using Services.API;
using Services.Navigation;
using Shared.Models;
using BaseViewModel = Framework.BaseViewModel;
using Location = Location;

public sealed partial class ExplorerViewModel(
	IPlacesApi placesApi,
	IGeolocation geoLocation,
	IDialogService dialogService,
	IDispatcher dispatcher,
	IDeviceDisplay deviceDisplay,
	INavigationService navigationService) : BaseViewModel, IDisposable
{
	private const string DefaultPinImage = "https://ik.imagekit.io/VladislavAntonyuk/projects/world-explorer/default-location-pin.png";

	[ObservableProperty]
	private Location? currentLocation;

	[ObservableProperty]
	private string? status;

	public ObservableRangeCollection<WorldExplorerPin> Pins { get; } = [];

	public void Dispose()
	{
		StopTracking();
	}

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

	[RelayCommand]
	private async Task MapReady()
	{
		await dispatcher.DispatchAsync(StartTracking);
	}

	[RelayCommand]
	private async Task ToggleUserLocation()
	{
		if (CurrentLocation is null)
		{
			await StartTracking();
		}
		else
		{
			StopTracking();
		}
	}

	[RelayCommand(AllowConcurrentExecutions = false)]
	private async Task MarkerClicked(WorldExplorerPin? pin)
	{
		if (pin is null)
		{
			return;
		}

		var taskCompletionSource = new TaskCompletionSource();
		await dispatcher.DispatchAsync(() => navigationService.NavigateAsync<PlaceDetailsViewModel, ErrorViewModel>(new Dictionary<string, object>
		{
			{
				"place", pin.PlaceId
			},
			{
				BasePopupViewModel.TaskCompletionSourceKey, taskCompletionSource
			}
		}));
		await taskCompletionSource.Task;
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

		deviceDisplay.KeepScreenOn = true;
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

	[RelayCommand]
	private void Refresh()
	{
		Status = Localization.LookingForPlaces;
		OnCurrentLocationChanged(CurrentLocation);
	}

	async partial void OnCurrentLocationChanged(Location? value)
	{
		if (value is null)
		{
			return;
		}

		var attempts = 0;
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

		if (!placesResponse.IsSuccessful)
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
				Pins.AddRange(placesResponse.Content.Result
											.Where(x => Pins.All(pin => pin.PlaceId != x.Id))
											.Select(place => new WorldExplorerPin
											{
												PlaceId = place.Id,
												Location = place.Location,
												Label = place.Name,
												Image = string.IsNullOrEmpty(place.MainImage)
													? DefaultPinImage
													: place.MainImage,
												MarkerClicked = new AsyncRelayCommand<WorldExplorerPin>(MarkerClicked)
											}));

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

	private void StopTracking()
	{
		geoLocation.ListeningFailed -= GeoLocationOnListeningFailed;
		geoLocation.LocationChanged -= GeoLocationOnLocationChanged;
		geoLocation.StopListeningForeground();
		CurrentLocation = null;
		deviceDisplay.KeepScreenOn = false;
	}
}