namespace WebApp.Components.Map;

using Components;
using Dialogs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Services.Place;
using Shared.Enums;
using Shared.Models;

public partial class WorldExplorerMap : WorldExplorerBaseComponent, IAsyncDisposable
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

	private async Task<StatusCode> AddNearbyPlaces(Location location)
	{
		var placesResult = await PlacesService.GetNearByPlaces(location, CancellationToken.None);
		switch (placesResult.StatusCode)
		{
			case StatusCode.Success:
				if (placesResult.Result.Count == 0)
				{
					Snackbar.Add(Translation.NoPlacesFoundNearby, Severity.Info);
					break;
				}

				foreach (var place in placesResult.Result)
				{
					await CreateMarker(place.Id, place.Location, place.Name, place.MainImage);
				}

				break;
			case StatusCode.LocationInfoRequestPending:
				Snackbar.Add(Translation.LoadingPlaces, Severity.Info, options => options.ShowCloseIcon = false);
				break;
		}

		return placesResult.StatusCode;
	}

	[JSInvokable]
	public async Task UpdatePosition(Location location)
	{
		if (currentLocation is null ||
			!PlacesService.IsNearby(currentLocation, location, DistanceConstants.LocationDistance))
		{
			currentLocation = location;
			isLoading = false;
			StateHasChanged();
			StatusCode statusCode;
			do
			{
				statusCode = await AddNearbyPlaces(currentLocation);
				await Task.Delay(TimeSpan.FromSeconds(10));
			} while (statusCode == StatusCode.LocationInfoRequestPending);
		}
	}

	[JSInvokable]
	public void UpdatePositionError(string message)
	{
		errorMessage = message;
	}

	[JSInvokable]
	public async Task OpenDetails(Guid id, string title)
	{
		var placeDetails = await PlacesService.GetPlaceDetails(id, CancellationToken.None);
		if (placeDetails is null)
		{
			Snackbar.Add($"{title} {Translation.NotFound}", Severity.Error);
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

	private async Task<Marker> CreateMarker(Guid id, Location location, string label, string? icon)
	{
		const string defaultIcon = "/assets/default-location-pin.png";
		var markerOptions = new MarkerOptions(location, label, icon ?? defaultIcon);
		var marker = new Marker(markerOptions);
		await JsRuntime.InvokeVoidAsync("leafletInterop.addMarker", mapRef, id, marker);
		return marker;
	}
}