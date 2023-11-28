namespace Client.Services.API;

using Refit;
using Shared.Models;

public interface IPlacesApi
{
	[Get("/recommendations")]
	internal Task<ApiResponse<OperationResult<List<Place>>>> GetRecommendationsInternal([Query] Location location,
		CancellationToken cancellationToken);

	async Task<ApiResponse<OperationResult<List<Place>>>> GetRecommendations([Query] Location location,
		CancellationToken cancellationToken)
	{
		try
		{
			return await GetRecommendationsInternal(location, cancellationToken);
		}
		catch (Exception e)
		{
			return await e.GetErrorResponse<OperationResult<List<Place>>>(HttpMethod.Get);
		}
	}

	[Get("/{id}")]
	internal Task<ApiResponse<Place>> GetDetailsInternal(Guid id, CancellationToken cancellationToken);

	async Task<ApiResponse<Place>> GetDetails(Guid id, CancellationToken cancellationToken)
	{
		try
		{
			return await GetDetailsInternal(id, cancellationToken);
		}
		catch (Exception e)
		{
			return await e.GetErrorResponse<Place>(HttpMethod.Get);
		}
	}
}