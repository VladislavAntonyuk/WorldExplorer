namespace Client.Services.API;

using Refit;
using Shared.Models;

public interface IPlacesApi
{
	[Get("/recommendations")]
	[Headers("Authorization: Bearer")]
	internal Task<ApiResponse<OperationResult<List<PlaceResponse>>>> GetRecommendationsInternal([Query] Location location,
		CancellationToken cancellationToken);

	async Task<ApiResponse<OperationResult<List<PlaceResponse>>>> GetRecommendations([Query] Location location,
		CancellationToken cancellationToken)
	{
		try
		{
			return await GetRecommendationsInternal(location, cancellationToken);
		}
		catch (Exception e)
		{
			return await e.GetErrorResponse<OperationResult<List<PlaceResponse>>>(HttpMethod.Get);
		}
	}

	[Get("/{id}")]
	[Headers("Authorization: Bearer")]
	internal Task<ApiResponse<PlaceResponse>> GetDetailsInternal(Guid id, CancellationToken cancellationToken);

	async Task<ApiResponse<PlaceResponse>> GetDetails(Guid id, CancellationToken cancellationToken)
	{
		try
		{
			return await GetDetailsInternal(id, cancellationToken);
		}
		catch (Exception e)
		{
			return await e.GetErrorResponse<PlaceResponse>(HttpMethod.Get);
		}
	}
}