namespace WebApp.Shared.Map;

using Dialogs;
using global::Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Services;

public partial class WorldExplorerMap : WorldExplorerAuthBaseComponent, IAsyncDisposable
{
	private Location? currentLocation;
	private string? errorMessage;
	private bool isLoading = true;
	private DotNetObjectReference<WorldExplorerMap>? mapRef;

	[Inject]
	public required IJSRuntime JsRuntime { get; set; }

	[Inject]
	public required IPlacesService PlacesService { get; set; }

	[Inject]
	public required IDialogService DialogService { get; set; }

	[Inject]
	public required ISnackbar Snackbar { get; set; }

	public async ValueTask DisposeAsync()
	{
		await JsRuntime.InvokeVoidAsync("leafletInterop.destroyMap");
		mapRef?.Dispose();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			mapRef = DotNetObjectReference.Create(this);
			await JsRuntime.InvokeVoidAsync("leafletInterop.initMap", mapRef, new MapOptions(null, 15));
		}
	}

	private async Task AddNearbyPlaces(Location location)
	{
		var places = await PlacesService.GetNearByPlaces(location, CancellationToken.None);
		if (places.Count == 0)
		{
			await DialogService.ShowMessageBox(string.Empty, "No places found nearby");
		}

		foreach (var place in places)
		{
			await CreateMarker(place.Location, place.Name, place.MainImage);
		}
	}

	[JSInvokable]
	public async Task UpdatePosition(Location location)
	{
		if (currentLocation is null || !PlacesService.IsNearby(currentLocation, location, DistanceConstants.LocationDistance))
		{
			currentLocation = location;
			isLoading = false;
			StateHasChanged();
			await AddNearbyPlaces(currentLocation);
		}
	}

	[JSInvokable]
	public void UpdatePositionError(string message)
	{
		errorMessage = message;
	}

	[JSInvokable]
	public async Task OpenDetails(string title, Location location)
	{
		var placeDetails = await PlacesService.GetPlaceDetails(title, location, CancellationToken.None);
		if (placeDetails is null)
		{
			Snackbar.Add($"{title} not found", Severity.Error);
		}
		else
		{
			await DialogService.ShowAsync<PlaceDetailsDialog>(placeDetails.Name, new DialogParameters
			{
				{
					nameof(PlaceDetailsDialog.Place), placeDetails
				}
			});
		}
	}

	private async Task<Marker> CreateMarker(Location location, string label, string? icon)
	{
		const string defaultIcon = "/assets/default-location-pin.png";
		var markerOptions = new MarkerOptions(location, label, icon ?? defaultIcon);
		var marker = new Marker(markerOptions);
		await JsRuntime.InvokeVoidAsync("leafletInterop.addMarker", mapRef, marker);
		return marker;
	}
}