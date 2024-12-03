namespace Client.Services.API;

using Models;
using Refit;

public interface IUsersApi
{
	[Delete("/profile")]
	[Headers("Authorization: Bearer")]
	internal Task<IApiResponse> DeleteInternal(CancellationToken cancellationToken);

	async Task<IApiResponse> Delete(CancellationToken cancellationToken)
	{
		try
		{
			return await DeleteInternal(cancellationToken);
		}
		catch (Exception e)
		{
			return await e.GetErrorResponse(HttpMethod.Delete);
		}
	}

	[Get("/profile")]
	[Headers("Authorization: Bearer")]
	internal Task<ApiResponse<User>> GetCurrentUserInternal(CancellationToken cancellationToken);

	async Task<ApiResponse<User>> GetCurrentUser(CancellationToken cancellationToken)
	{
		try
		{
			return await GetCurrentUserInternal(cancellationToken);
		}
		catch (Exception e)
		{
			return await e.GetErrorResponse<User>(HttpMethod.Get);
		}
	}

	[Put("/profile")]
	[Headers("Authorization: Bearer")]
	internal Task<IApiResponse> UpdateCurrentUserInternal(UpdateUserRequest user, CancellationToken cancellationToken);

	async Task<IApiResponse> UpdateCurrentUser(UpdateUserRequest user, CancellationToken cancellationToken)
	{
		try
		{
			return await UpdateCurrentUserInternal(user, cancellationToken);
		}
		catch (Exception e)
		{
			return await e.GetErrorResponse(HttpMethod.Put);
		}
	}

	public sealed class UpdateUserRequest
	{
		public bool TrackUserLocation { get; set; }
	}
}