namespace Client.Services;

using API;
using Refit;
using Shared.Models;

public interface IPlacesApi
{
	[Get("/recommendations")]
	internal Task<ApiResponse<List<Place>>> GetRecommendationsInternal([Query] Location location,
		CancellationToken cancellationToken);

	async Task<ApiResponse<List<Place>>> GetRecommendations([Query] Location location,
		CancellationToken cancellationToken)
	{
		try
		{
			return await GetRecommendationsInternal(location, cancellationToken);
		}
		catch (Exception e)
		{
			return await e.GetErrorResponse<List<Place>>(HttpMethod.Get);
		}
	}

	[Get("/details")]
	internal Task<ApiResponse<Place>> GetDetailsInternal([Query] string name,
		[Query] Location location,
		CancellationToken cancellationToken);

	async Task<ApiResponse<Place>> GetDetails([Query] string name,
		[Query] Location location,
		CancellationToken cancellationToken)
	{
		try
		{
			return await GetDetailsInternal(name, location, cancellationToken);
		}
		catch (Exception e)
		{
			return await e.GetErrorResponse<Place>(HttpMethod.Get);
		}
	}
}