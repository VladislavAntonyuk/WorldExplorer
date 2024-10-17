namespace WorldExplorer.Web;

using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Modules.Places.Application.Abstractions;
using Modules.Places.Application.Places.GetPlace;
using Modules.Users.Application.Users.GetUser;
using WorldExplorer.Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequests;

public class WorldExplorerApiClient(HttpClient httpClient)
{
	public async Task ClearPlaces(CancellationToken none)
	{
		await httpClient.DeleteAsync("places", none);
	}

	public async Task ClearLocationInfoRequests(CancellationToken none)
	{
		await httpClient.DeleteAsync("locationInfoRequests", none);
	}

	public async Task<List<UserResponse>> GetUsers(CancellationToken none)
	{
		return await httpClient.GetFromJsonAsync<List<UserResponse>>("users", none) ?? [];
	}

	public async Task<List<PlaceResponse>> GetPlaces(CancellationToken none)
	{
		return await httpClient.GetFromJsonAsync<List<PlaceResponse>>("places", none) ?? [];
	}

	public async Task<List<LocationInfoRequestResponse>> GetLocationInfoRequests(CancellationToken none)
	{
		return await httpClient.GetFromJsonAsync<List<LocationInfoRequestResponse>>("locationInfoRequests", none) ?? [];
	}

	public async Task DeleteUser(CancellationToken none)
	{
		//self delete
		await httpClient.DeleteAsync("users/profile", none);
	}

	public async Task DeleteUser(Guid userId, CancellationToken none)
	{
		// admin delete
		await httpClient.DeleteAsync($"users/{userId}", none);
	}

	public async Task DeletePlace(Guid placeId, CancellationToken none)
	{
		// admin delete
		await httpClient.DeleteAsync($"places/{placeId}", none);
	}

	public async Task DeleteLocationInfoRequest(int requestId, CancellationToken none)
	{
		// admin delete
		await httpClient.DeleteAsync($"locationInfoRequests/{requestId}", none);
	}

	public async Task<PlaceResponse?> GetPlaceDetails(Guid placeId, CancellationToken none)
	{
		return await httpClient.GetFromJsonAsync<PlaceResponse>($"places/{placeId}", none);
	}

	public async Task UpdatePlace(Guid id, PlaceRequest place, CancellationToken none)
	{
		await httpClient.PutAsJsonAsync($"places/{id}", place, none);
	}

	public Task<UserResponse?> GetUser(string providerId, CancellationToken none)
	{
		//admin get
		return httpClient.GetFromJsonAsync<UserResponse>($"users/{providerId}", none);
	}

	public Task<UserResponse?> GetCurrentUser(CancellationToken none)
	{
		return httpClient.GetFromJsonAsync<UserResponse>("users/profile", none);
	}

	public Task UpdateUser(bool trackUserLocation, CancellationToken none)
	{
		var request = new
		{
			TrackUserLocation = trackUserLocation
		};
		return httpClient.PutAsJsonAsync("users/profile", request, none);
	}

	public async Task<OperationResult<List<PlaceResponse>>> GetNearByPlaces(Location location, CancellationToken none)
	{
		var r = await httpClient.GetFromJsonAsync<OperationResult<List<PlaceResponse>>>(
			$"places/recommendations?Latitude={location.Latitude}&Longitude={location.Longitude}", none);
		return r ??
		       new OperationResult<List<PlaceResponse>>
		       {
			       StatusCode = StatusCode.LocationInfoRequestPending
		       };
	}
}

public class OperationResult<T> where T : new()
{
	public T Result { get; init; } = new();

	public StatusCode StatusCode { get; init; }
}

public enum StatusCode
{
	Success,
	LocationInfoRequestPending,
	FailedResponse
}