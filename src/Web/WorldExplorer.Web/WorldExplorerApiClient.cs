namespace WorldExplorer.Web;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Abstractions;
using Modules.Places.Application.Places.GetPlace;
using Modules.Places.Domain.Places;
using Modules.Users.Application.Users.GetUser;
using System.Net.Http.Headers;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

public class ApiService
{
	private readonly HttpClient _httpClient;
	private readonly ITokenAcquisition _tokenAcquisition;

	public ApiService(HttpClient httpClient, ITokenAcquisition tokenAcquisition)
	{
		_httpClient = httpClient;
		_tokenAcquisition = tokenAcquisition;
	}

	public async Task<string> GetProtectedDataAsync()
	{
		try
		{
			var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "https://drawgo.onmicrosoft.com/3c3bdb4b-327b-49a9-a13e-0b565526b8a1/.default" },
				JwtBearerDefaults.AuthenticationScheme, "b1b086fc-3602-4b9d-b8d3-cd1669375e87", "B2C_1_WorldExplorer_SIGNUP_SIGNIN", ClaimsPrincipal.Current, new TokenAcquisitionOptions()
				{
					AuthenticationOptionsName = JwtBearerDefaults.AuthenticationScheme,
					ManagedIdentity = new ManagedIdentityOptions(){UserAssignedClientId = "3c3bdb4b-327b-49a9-a13e-0b565526b8a1"},
					UserFlow = "B2C_1_WorldExplorer_SIGNUP_SIGNIN",
					Tenant = "b1b086fc-3602-4b9d-b8d3-cd1669375e87",
				});

			_httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", accessToken);

			return await _httpClient.GetStringAsync("api/MyProtected");

		}
		catch (Exception e)
		{
			return await _httpClient.GetStringAsync("api/MyProtected");
		}
	}
}

public class WorldExplorerApiClient(IDownstreamApi downstreamApi, ApiService apiService)
{
	public async Task ClearPlaces(CancellationToken none)
	{
		
	}

	public async Task ClearLocationInfoRequests(CancellationToken none)
	{
		
	}

	public async Task<List<UserResponse>> GetUsers(CancellationToken none)
	{
		return [];
	}

	public async Task<List<PlaceResponse>> GetPlaces(CancellationToken none)
	{
		return [];
	}

	public async Task<List<LocationInfoRequestResponse>> GetLocationInfoRequests(CancellationToken none)
	{
		return [];
	}

	public async Task DeleteUser(string providerId, CancellationToken none)
	{
		//self delete
		
	}

	public async Task DeleteUser(Guid userId, CancellationToken none)
	{
		// admin delete
	}

	public async Task DeletePlace(Guid placeId, CancellationToken none)
	{
	}

	public async Task DeleteLocationInfoRequest(Guid requestId, CancellationToken none)
	{
	}

	public async Task<PlaceResponse?> GetPlaceDetails(Guid placeId, CancellationToken none)
	{
		return new PlaceResponse(placeId, "1", ",", Location.Default, 1, []);
	}

	//ToDo should be place request
	public async Task UpdatePlace(PlaceResponse place, CancellationToken none)
	{

	}

	public async Task<UserResponse?> GetUser(string providerId, CancellationToken none)
	{
		//admin get
		try
		{
			var d = await apiService.GetProtectedDataAsync();
			var c = await downstreamApi.GetForUserAsync<string>("WorldExplorerApiClient", options =>
			{
				options.AcquireTokenOptions.AuthenticationOptionsName = JwtBearerDefaults.AuthenticationScheme;
				options.AcquireTokenOptions.ManagedIdentity = new ManagedIdentityOptions();
				options.AcquireTokenOptions.ManagedIdentity.UserAssignedClientId = "3c3bdb4b-327b-49a9-a13e-0b565526b8a1";
				options.AcquireTokenOptions.UserFlow = "B2C_1_WorldExplorer_SIGNUP_SIGNIN";
				options.AcquireTokenOptions.Tenant = "b1b086fc-3602-4b9d-b8d3-cd1669375e87";
				options.RelativePath = $"api/myprotected";
			}, ClaimsPrincipal.Current, none);
		}
		catch (Exception e)
		{
			
		}
		return await downstreamApi.GetForUserAsync<UserResponse>("WorldExplorerApiClient", options =>
		{
			options.AcquireTokenOptions.AuthenticationOptionsName = JwtBearerDefaults.AuthenticationScheme;
			options.AcquireTokenOptions.ManagedIdentity = new ManagedIdentityOptions();
			options.AcquireTokenOptions.ManagedIdentity.UserAssignedClientId = "3c3bdb4b-327b-49a9-a13e-0b565526b8a1";
			options.AcquireTokenOptions.UserFlow = "B2C_1_WorldExplorer_SIGNUP_SIGNIN";
			options.AcquireTokenOptions.Tenant = "b1b086fc-3602-4b9d-b8d3-cd1669375e87";
			options.RelativePath = $"users/profile";
		}, ClaimsPrincipal.Current, none);
		return await downstreamApi.GetForUserAsync<UserResponse>("WorldExplorerApiClient", options =>
		{
			options.RelativePath = $"users/{providerId}";
		}, ClaimsPrincipal.Current, none);
	}

	public async Task<UserResponse?> GetCurrentUser(CancellationToken none)
	{
		return await downstreamApi.GetForUserAsync<UserResponse>("WorldExplorerApiClient", options =>
		{
			options.RelativePath = $"users/profile";
		}, ClaimsPrincipal.Current, none);
	}

	// toDo should be user request
	public async Task UpdateUser(UserResponse user, CancellationToken none)
	{
	}

	public async Task<OperationResult<List<PlaceResponse>>> GetNearByPlaces(Location location, CancellationToken none)
	{
		return new OperationResult<List<PlaceResponse>>()
		{
			Result = new List<PlaceResponse>(),
			StatusCode = StatusCode.Success
		};
	}

	public bool IsNearby(Location currentLocation, Location location)
	{
		return true;
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