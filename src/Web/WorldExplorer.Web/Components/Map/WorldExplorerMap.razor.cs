namespace WorldExplorer.Web.Components.Map;

using Dialogs;
using Microsoft.JSInterop;
using Modules.Places.Application.Abstractions;
using MudBlazor;
using User;

public sealed partial class WorldExplorerMap(IJSRuntime jsRuntime,
	WorldExplorerApiClient apiClient,
	ICurrentUserService currentUserService,
	IDialogService dialogService,
	ISnackbar snackbar) : WorldExplorerBaseComponent, IAsyncDisposable
{
	private Location? currentLocation;
	private string? errorMessage;
	private bool isLoading = true;
	private DotNetObjectReference<WorldExplorerMap>? mapRef;

	public async ValueTask DisposeAsync()
	{
		//await jsRuntime.InvokeVoidAsync("leafletInterop.destroyMap");
		//mapRef?.Dispose();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			var user = await apiClient.GetCurrentUser(CancellationToken.None);
			mapRef = DotNetObjectReference.Create(this);
			await jsRuntime.InvokeVoidAsync("leafletInterop.initMap", mapRef, new MapOptions(null, 15)
			{
				TrackUserLocation = user?.Settings.TrackUserLocation == true
			});
		}
	}

	private async Task<StatusCode> AddNearbyPlaces(Location location)
	{
		var placesResult = await apiClient.GetNearByPlaces(location, CancellationToken.None);
		switch (placesResult.StatusCode)
		{
			case StatusCode.Success:
				if (placesResult.Result.Count == 0)
				{
					snackbar.Add(Translation.NoPlacesFoundNearby, Severity.Info);
					break;
				}

				foreach (var place in placesResult.Result)
				{
					await CreateMarker(place.Id, place.Location, place.Name, place.MainImage);
				}

				break;
			case StatusCode.LocationInfoRequestPending:
				snackbar.Add(Translation.LoadingPlaces, Severity.Info, options => options.ShowCloseIcon = false);
				break;
		}

		return placesResult.StatusCode;
	}

	[JSInvokable]
	public async Task UpdatePosition(Location location)
	{
		if (currentLocation is null || !apiClient.IsNearby(currentLocation, location))
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
		await dialogService.ShowAsync<PlaceDetailsDialog>(title, new DialogParameters
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
		await jsRuntime.InvokeVoidAsync("leafletInterop.addMarker", mapRef, id, marker);
		return marker;
	}
}