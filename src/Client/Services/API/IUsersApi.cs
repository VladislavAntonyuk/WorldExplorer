namespace Client.Services;

using API;
using Refit;
using Shared.Models;

public interface IUsersApi
{
	[Delete("/self")]
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

	[Get("/self")]
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
}