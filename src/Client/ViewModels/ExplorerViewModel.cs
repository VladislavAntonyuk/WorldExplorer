namespace Client.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls;
using Framework;
using Microsoft.Maui.Controls.Maps;
using Models;
using Resources.Localization;
using Services;
using Services.API;
using Shared.Enums;
using Views;
using Location = Microsoft.Maui.Devices.Sensors.Location;

public sealed partial class ExplorerViewModel(IPlacesApi placesApi,
	ILauncher launcher,
	IGeolocator geoLocator,
	IDialogService dialogService,
	IDispatcher dispatcher,
	IDeviceDisplay deviceDisplay) : BaseViewModel, IDisposable
{
	[ObservableProperty]
	private bool isShowingUser = true;

	[ObservableProperty]
	private GeolocatorData? currentGeolocatorData;

	private void GeoLocator_PositionChanged(object? sender, GeolocatorData e)
	{
		var moveToRegion = CurrentGeolocatorData == null;
		CurrentGeolocatorData = e;
		weakEventManager.HandleEvent(this, new LocationChangedEventArgs
		{
			Location = e.Location,
			MoveToRegion = moveToRegion
		}, nameof(LocationChanged));
	}

	public ObservableCollection<WorldExplorerPin> Pins { get; } = [];

	private readonly WeakEventManager weakEventManager = new();
	public event EventHandler<LocationChangedEventArgs> LocationChanged
	{
		add => weakEventManager.AddEventHandler(value);
		remove => weakEventManager.RemoveEventHandler(value);
	}

	public override async Task InitializeAsync()
	{
		geoLocator.PositionChanged += GeoLocator_PositionChanged;
		deviceDisplay.KeepScreenOn = true;
		await base.InitializeAsync();
		await StartTracking();
	}

	public override Task UnInitializeAsync()
	{
		geoLocator.PositionChanged -= GeoLocator_PositionChanged;
		geoLocator.StopListening();
		deviceDisplay.KeepScreenOn = false;
		return base.UnInitializeAsync();
	}

	[RelayCommand]
	private void ToggleUserLocation()
	{
		IsShowingUser = !IsShowingUser;
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
	private async Task StartTracking()
	{
		var permission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
		if (permission != PermissionStatus.Granted)
		{
			await dialogService.ToastAsync("No permission");
			return;
		}

		await dialogService.ToastAsync(Localization.LookingForPlaces);
		geoLocator.StartListening();
	}

	async partial void OnCurrentGeolocatorDataChanged(GeolocatorData? value)
	{
		if (value is null)
		{
			return;
		}

		StatusCode statusCode;
		do
		{
			statusCode = await GetRecommendations(value);
		} while (statusCode == StatusCode.LocationInfoRequestPending);
	}

	private async Task<StatusCode> GetRecommendations(GeolocatorData value)
	{
		var placesResponse = await placesApi.GetRecommendations(new Shared.Models.Location(value.Location.Latitude, value.Location.Longitude), CancellationToken.None);

		if (!placesResponse.IsSuccessStatusCode)
		{
			await dialogService.ToastAsync(placesResponse.Error.Message);
			return StatusCode.FailedResponse;
		}

		switch (placesResponse.Content.StatusCode)
		{
			case StatusCode.Success:
				if (placesResponse.Content.Result.Count == 0)
				{
					await dialogService.ToastAsync(Localization.NoPlacesFound);
					break;
				}

				await dispatcher.DispatchAsync(async () =>
				{
					foreach (var place in placesResponse.Content.Result.Where(x => Pins.All(pin => pin.PlaceId != x.Id)))
					{
						Pins.Add(new WorldExplorerPin()
						{
							PlaceId = place.Id,
							Location = new Location(place.Location.Latitude, place.Location.Longitude),
							Label = place.Name,
							Type = PinType.Place,
							Address = place.Description ?? string.Empty
						});
					}

					await CheckLocation(value.Location);
				});

				break;
			case StatusCode.LocationInfoRequestPending:
				await dialogService.ToastAsync(Localization.LookingForPlaces);
				await Task.Delay(TimeSpan.FromSeconds(10));
				break;
		}

		return placesResponse.Content.StatusCode;
	}

	private async Task CheckLocation(Location location)
	{
		Pin? closestPlace = null;
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
		geoLocator.PositionChanged -= GeoLocator_PositionChanged;
		geoLocator.StopListening();
	}
}