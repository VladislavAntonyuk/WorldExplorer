namespace WebApp.Components.Map;

using Dialogs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Services.Place;
using Services.User;
using Shared.Enums;
using Shared.Models;

public sealed partial class WorldExplorerMap : WorldExplorerBaseComponent, IAsyncDisposable
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

	[Inject]
	public required ICurrentUserService CurrentUserService { get; set; }

	[Inject]
	public required IUserService UserService { get; set; }

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
			var currentUser = CurrentUserService.GetCurrentUser();
			var user = await UserService.GetUser(currentUser.ProviderId, CancellationToken.None);
			mapRef = DotNetObjectReference.Create(this);
			await JsRuntime.InvokeVoidAsync("leafletInterop.initMap", mapRef, new MapOptions(null, 15)
			{
				TrackUserLocation = user?.Settings.TrackUserLocation == true
			});
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
		if (currentLocation is null || !PlacesService.IsNearby(currentLocation, location))
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
		await DialogService.ShowAsync<PlaceDetailsDialog>(title, new DialogParameters
		{
			{
				nameof(PlaceDetailsDialog.PlaceId), id
			}
		});
	}

	private async Task<Marker> CreateMarker(Guid id, Location location, string label, string? icon)
	{
		const string defaultIcon = "/assets/default-location-pin.png";
		var marker = new Marker(location, label, icon ?? defaultIcon);
		await JsRuntime.InvokeVoidAsync("leafletInterop.addMarker", mapRef, id, marker);
		return marker;
	}
}