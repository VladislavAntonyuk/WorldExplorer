namespace Client.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Microsoft.Maui.Controls.Maps;
using Services;

public partial class ExplorerViewModel : BaseViewModel
{
	private readonly IDialogService dialogService;
	private readonly IGeolocator geoLocator;

	private readonly IPlacesApi placesApi;

	[ObservableProperty]
	private Location? currentLocation;

	public ExplorerViewModel(IPlacesApi placesApi, IGeolocator geoLocator, IDialogService dialogService)
	{
		this.placesApi = placesApi;
		this.geoLocator = geoLocator;
		this.dialogService = dialogService;
	}

	public ObservableCollection<Pin> Pins { get; } = new();

	public event EventHandler<LocationChangedEventArgs>? LocationChanged;

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		await StartTracking(CancellationToken.None);
	}

	[RelayCommand(AllowConcurrentExecutions = false)]
	private async Task StartTracking(CancellationToken cancellationToken)
	{
		var progress = new Progress<Location>(location =>
		{
			CurrentLocation = location;
			LocationChanged?.Invoke(this, new LocationChangedEventArgs
			{
				Location = location
			});
		});
		await geoLocator.StartListening(progress, cancellationToken);
	}

	async partial void OnCurrentLocationChanged(Location? value)
	{
		if (value is null)
		{
			return;
		}

		var placesResponse =
			await placesApi.GetRecommendations(new Shared.Models.Location(value.Latitude, value.Longitude),
			                                   CancellationToken.None);
		if (!placesResponse.IsSuccessStatusCode)
		{
			await dialogService.ToastAsync(placesResponse.Error.Message);
			return;
		}

		if (placesResponse.Content.Count == 0)
		{
			await dialogService.ToastAsync("No places found");
			return;
		}

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

		await CheckLocation(value);
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
			await dialogService.ToastAsync($"You are near {closestPlace.Label}", CancellationToken.None);
		}
	}
}

public class LocationChangedEventArgs : EventArgs
{
	public required Location Location { get; init; }
}