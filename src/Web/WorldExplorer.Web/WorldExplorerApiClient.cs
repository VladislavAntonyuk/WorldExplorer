namespace WorldExplorer.Web;

using Microsoft.Identity.Abstractions;
using Modules.Places.Application.Places.GetPlace;
using Modules.Places.Domain.Places;
using Modules.Users.Application.Users.GetUser;
using Microsoft.Identity.Web;
using Modules.Places.Application.Abstractions;

public class WorldExplorerApiClient(IDownstreamApi downstreamApi, MicrosoftIdentityConsentAndConditionalAccessHandler handler)
{
	public async Task ClearPlaces(CancellationToken none)
	{
		await DeleteAsync("places", none);
	}

	public async Task ClearLocationInfoRequests(CancellationToken none)
	{
		await DeleteAsync("locationInfoRequests", none);
	}

	public async Task<List<UserResponse>> GetUsers(CancellationToken none)
	{
		return await GetAsync<List<UserResponse>>("users", none) ?? [];
	}

	public async Task<List<PlaceResponse>> GetPlaces(CancellationToken none)
	{
		return await GetAsync<List<PlaceResponse>>("places", none) ?? [];
	}

	public async Task<List<LocationInfoRequestResponse>> GetLocationInfoRequests(CancellationToken none)
	{
		return await GetAsync<List<LocationInfoRequestResponse>>("locationInfoRequests", none) ?? [];
	}

	public async Task DeleteUser(CancellationToken none)
	{
		//self delete
		await DeleteAsync("users/profile", none);
	}

	public async Task DeleteUser(Guid userId, CancellationToken none)
	{
		// admin delete
		await DeleteAsync($"users/{userId}", none);
	}

	public async Task DeletePlace(Guid placeId, CancellationToken none)
	{
		// admin delete
		await DeleteAsync($"places/{placeId}", none);
	}

	public async Task DeleteLocationInfoRequest(Guid requestId, CancellationToken none)
	{
		// admin delete
		await DeleteAsync($"locationInfoRequestsw/{requestId}", none);
	}

	public async Task<PlaceResponse?> GetPlaceDetails(Guid placeId, CancellationToken none)
	{
		return await GetAsync<PlaceResponse>($"places/{placeId}", none);
	}

	//ToDo should be place request
	public async Task UpdatePlace(PlaceResponse place, CancellationToken none)
	{

	}

	public Task<UserResponse?> GetUser(string providerId, CancellationToken none)
	{
		//admin get
		return GetAsync<UserResponse>($"users/{providerId}", none);
	}

	public Task<UserResponse?> GetCurrentUser(CancellationToken none)
	{
		return GetAsync<UserResponse>("users/profile", none);
	}

	public Task UpdateUser(bool trackUserLocation, CancellationToken none)
	{
		var request = new
		{
			TrackUserLocation = trackUserLocation
		};
		return PutAsync("users/profile", request, none);
	}

	public async Task<OperationResult<List<PlaceResponse>>> GetNearByPlaces(Location location, CancellationToken none)
	{
		var r = await GetAsync<OperationResult<List<PlaceResponse>>>($"places/recommendations?Latitude={location.Latitude}&Longitude={location.Longitude}", none);
		return r ?? new OperationResult<List<PlaceResponse>>()
		{
			StatusCode = StatusCode.LocationInfoRequestPending
		};
	}

	public bool IsNearby(Location currentLocation, Location location)
	{
		return false;
	}

	private async Task DeleteAsync(string url, CancellationToken cancellationToken)
	{
		try
		{
			await downstreamApi.CallApiForUserAsync("WorldExplorerApiClient", options =>
			{
				options.RelativePath = url;
				options.HttpMethod = HttpMethod.Delete.Method;
			}, cancellationToken: cancellationToken);
		}
		catch (Exception e)
		{
			handler.HandleException(e);
		}
	}

	private async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken)
		where T : class
	{
		try
		{
			return await downstreamApi.CallApiForUserAsync<T>("WorldExplorerApiClient", options =>
			{
				options.RelativePath = url;
				options.HttpMethod = HttpMethod.Get.Method;
			}, cancellationToken: cancellationToken);
		}
		catch (Exception e)
		{
			handler.HandleException(e);
		}

		return default;
	}

	private async Task<TOutput?> PostAsync<TInput, TOutput>(string url, TInput content, CancellationToken cancellationToken)
		where TInput : class
		where TOutput : class
	{
		try
		{
			return await downstreamApi.PostForUserAsync<TInput, TOutput>("WorldExplorerApiClient", content, options =>
			{
				options.RelativePath = url;
			}, cancellationToken: cancellationToken);
		}
		catch (Exception e)
		{
			handler.HandleException(e);
		}

		return default;
	}

	private async Task PutAsync<TInput>(string url, TInput content, CancellationToken cancellationToken)
		where TInput : class
	{
		try
		{
			await downstreamApi.PutForUserAsync("WorldExplorerApiClient", content, options =>
			{
				options.RelativePath = url;
			}, cancellationToken: cancellationToken);
		}
		catch (Exception e)
		{
			handler.HandleException(e);
		}
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