namespace Client.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Microsoft.Maui.Controls.Maps;
using Models;
using Resources.Localization;
using Services;

public sealed partial class ExplorerViewModel : BaseViewModel, IDisposable
{
	private readonly IDeviceDisplay deviceDisplay;
	private readonly IDialogService dialogService;
	private readonly IDispatcher dispatcher;
	private readonly IGeolocator geoLocator;

	private readonly IPlacesApi placesApi;

	[ObservableProperty]
	private GeolocatorData? currentGeolocatorData;

	public ExplorerViewModel(IPlacesApi placesApi,
		IGeolocator geoLocator,
		IDialogService dialogService,
		IDispatcher dispatcher,
		IDeviceDisplay deviceDisplay)
	{
		this.placesApi = placesApi;
		this.geoLocator = geoLocator;
		geoLocator.PositionChanged += GeoLocator_PositionChanged;
		this.dialogService = dialogService;
		this.dispatcher = dispatcher;
		this.deviceDisplay = deviceDisplay;
	}

	private void GeoLocator_PositionChanged(object? sender, GeolocatorData e)
	{
		CurrentGeolocatorData = e;
		LocationChanged?.Invoke(this, new LocationChangedEventArgs
		{
			Location = e.Location
		});
	}

	public ObservableCollection<Pin> Pins { get; } = new();

	public event EventHandler<LocationChangedEventArgs>? LocationChanged;

	public override async Task InitializeAsync()
	{
		deviceDisplay.KeepScreenOn = true;
		await base.InitializeAsync();
		await StartTracking(CancellationToken.None);
	}

	public override Task UnInitializeAsync()
	{
		geoLocator.StopListening();
		deviceDisplay.KeepScreenOn = false;
		return base.UnInitializeAsync();
	}

	[RelayCommand(AllowConcurrentExecutions = false)]
	private async Task StartTracking(CancellationToken cancellationToken)
	{
		var permission = await Permissions.RequestAsync<Permissions.LocationAlways>();
		if (permission != PermissionStatus.Granted)
		{
			await dialogService.ToastAsync("No permission", CancellationToken.None);
			return;
		}

		await dialogService.ToastAsync(Localization.LookingForPlaces, cancellationToken);
		geoLocator.StartListening();
	}

	async partial void OnCurrentGeolocatorDataChanged(GeolocatorData? value)
	{
		if (value is null)
		{
			return;
		}

		var placesResponse = await placesApi.GetRecommendations(
			new Shared.Models.Location(value.Location.Latitude, value.Location.Longitude), CancellationToken.None);

		if (!placesResponse.IsSuccessStatusCode)
		{
			await dialogService.ToastAsync(placesResponse.Error.Message);
			return;
		}

		if (placesResponse.Content.Count == 0)
		{
			await dialogService.ToastAsync(Localization.NoPlacesFound);
			return;
		}

		dispatcher.Dispatch((() =>
		{
			foreach (var place in placesResponse.Content)
			{
				Pins.Add(new Pin
				{
					Location = new Location(place.Location.Latitude, place.Location.Longitude),
					Label = place.Name,
					Type = PinType.Place,
					Address = place.Description ?? string.Empty
				});
			}
		}));

		await CheckLocation(value.Location);
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
	}
}