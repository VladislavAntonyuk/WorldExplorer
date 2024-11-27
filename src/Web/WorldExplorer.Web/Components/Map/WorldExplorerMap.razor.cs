namespace WorldExplorer.Web.Components.Map;

using Dialogs;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Modules.Places.Application.Abstractions;
using Modules.Places.Application.Places.GetPlace;
using MudBlazor;

public sealed partial class WorldExplorerMap(
	IJSRuntime jsRuntime,
	WorldExplorerApiClient apiClient,
	IDialogService dialogService,
	ISnackbar snackbar,
	IOptions<PlacesSettings> placeOptions) : WorldExplorerBaseComponent, IAsyncDisposable
{
	private CancellationTokenSource cancellationTokenSource = new();
	private Location? currentLocation;
	private string? errorMessage;
	private bool isLoading = true;
	private bool isRendered;
	private DotNetObjectReference<WorldExplorerMap>? mapRef;

	public async ValueTask DisposeAsync()
	{
		if (isRendered)
		{
			await cancellationTokenSource.CancelAsync();
			await jsRuntime.InvokeVoidAsyncIgnoreErrors("leafletInterop.destroyMap");
			mapRef?.Dispose();
			cancellationTokenSource.Dispose();
			isRendered = false;
		}
	}

	protected override void OnInitialized()
	{
		cancellationTokenSource = new CancellationTokenSource();
		base.OnInitialized();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		isRendered = true;
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender && !cancellationTokenSource.IsCancellationRequested)
		{
			var user = await apiClient.GetCurrentUser(cancellationTokenSource.Token);
			mapRef = DotNetObjectReference.Create(this);
			await jsRuntime.InvokeVoidAsyncIgnoreErrors("leafletInterop.initMap", cancellationTokenSource.Token, mapRef,
			                                            new MapOptions(null, 15)
			                                            {
				                                            TrackUserLocation = user?.Settings.TrackUserLocation == true
			                                            });
		}
	}

	private async Task<StatusCode> AddNearbyPlaces(Location location)
	{
		var placesResult = await apiClient.GetNearByPlaces(location, cancellationTokenSource.Token);
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
					await CreateMarker(place);
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
		if (currentLocation is null ||
		    currentLocation.CalculateDistanceInMetersTo(location) > placeOptions.Value.LocationDistance)
		{
			currentLocation = location;
			isLoading = false;
			StateHasChanged();
			StatusCode statusCode;
			do
			{
				statusCode = await AddNearbyPlaces(currentLocation);
				await Task.Delay(TimeSpan.FromSeconds(10), cancellationTokenSource.Token);
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

	private async Task<Marker> CreateMarker(PlaceResponse place)
	{
		var distanceToPlace = currentLocation?.CalculateDistanceInMetersTo(place.Location);
		var title = distanceToPlace.HasValue ? $"{place.Name} ({distanceToPlace} {Translation.Meters})" : place.Name;
		const string defaultIcon = "/assets/default-location-pin.png";
		var marker = new Marker(place.Location, title, place.MainImage ?? defaultIcon);
		await jsRuntime.InvokeVoidAsyncIgnoreErrors("leafletInterop.addMarker", cancellationTokenSource.Token, mapRef, place.Id, marker);
		return marker;
	}
}