﻿namespace WorldExplorer.Web.Services;

using Microsoft.Identity.Web;
using Modules.Places.Application.Abstractions;
using Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequests;
using Modules.Places.Application.Places.GetPlace;
using Modules.Users.Application.Users.GetUser;

public class WorldExplorerApiClient(HttpClient httpClient, MicrosoftIdentityConsentAndConditionalAccessHandler handler)
{
	public async Task ClearPlaces(CancellationToken cancellationToken)
	{
		await httpClient.DeleteAsync("places", cancellationToken).Safe(handler.HandleException);
	}

	public async Task ClearLocationInfoRequests(CancellationToken cancellationToken)
	{
		await httpClient.DeleteAsync("locationInfoRequests", cancellationToken).Safe(handler.HandleException);
	}

	public async Task<List<UserResponse>> GetUsers(CancellationToken cancellationToken)
	{
		return await httpClient.GetFromJsonAsync<List<UserResponse>>("users", cancellationToken).Safe([], handler.HandleException) ?? [];
	}

	public async Task<List<PlaceResponse>> GetPlaces(CancellationToken cancellationToken)
	{
		return await httpClient.GetFromJsonAsync<List<PlaceResponse>>("places", cancellationToken).Safe([], handler.HandleException) ?? [];
	}

	public async Task<List<LocationInfoRequestResponse>> GetLocationInfoRequests(CancellationToken cancellationToken)
	{
		return await httpClient.GetFromJsonAsync<List<LocationInfoRequestResponse>>("locationInfoRequests", cancellationToken).Safe([], handler.HandleException) ?? [];
	}

	public async Task DeleteUser(CancellationToken cancellationToken)
	{
		//self delete
		await httpClient.DeleteAsync("users/profile", cancellationToken).Safe(handler.HandleException);
	}

	public async Task DeleteUser(Guid userId, CancellationToken cancellationToken)
	{
		// admin delete
		await httpClient.DeleteAsync($"users/{userId}", cancellationToken).Safe(handler.HandleException);
	}

	public async Task DeletePlace(Guid placeId, CancellationToken cancellationToken)
	{
		// admin delete
		await httpClient.DeleteAsync($"places/{placeId}", cancellationToken).Safe(handler.HandleException);
	}

	public async Task DeleteLocationInfoRequest(int requestId, CancellationToken cancellationToken)
	{
		// admin delete
		await httpClient.DeleteAsync($"locationInfoRequests/{requestId}", cancellationToken).Safe(handler.HandleException);
	}

	public async Task<PlaceResponse?> GetPlaceDetails(Guid placeId, CancellationToken cancellationToken)
	{
		return await httpClient.GetFromJsonAsync<PlaceResponse>($"places/{placeId}", cancellationToken).Safe(null, handler.HandleException);
	}

	public async Task UpdatePlace(Guid id, PlaceRequest place, CancellationToken cancellationToken)
	{
		await httpClient.PutAsJsonAsync($"places/{id}", place, cancellationToken).Safe(handler.HandleException);
	}

	public Task<UserResponse?> GetUser(string providerId, CancellationToken cancellationToken)
	{
		//admin get
		return httpClient.GetFromJsonAsync<UserResponse>($"users/{providerId}", cancellationToken).Safe(null, handler.HandleException);
	}

	public Task<UserResponse?> GetCurrentUser(CancellationToken cancellationToken)
	{
		return httpClient.GetFromJsonAsync<UserResponse>("users/profile", cancellationToken).Safe(null, handler.HandleException);
	}

	public Task UpdateUser(bool trackUserLocation, CancellationToken cancellationToken)
	{
		var request = new
		{
			TrackUserLocation = trackUserLocation
		};
		return httpClient.PutAsJsonAsync("users/profile", request, cancellationToken).Safe(handler.HandleException);
	}

	public async Task<OperationResult<List<PlaceResponse>>> GetNearByPlaces(Location location, CancellationToken cancellationToken)
	{
		var r = await httpClient.GetFromJsonAsync<OperationResult<List<PlaceResponse>>>(
			$"places/recommendations?Latitude={location.Latitude}&Longitude={location.Longitude}", cancellationToken).Safe(new OperationResult<List<PlaceResponse>>
		{
			StatusCode = StatusCode.LocationInfoRequestPending
		}, handler.HandleException);
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