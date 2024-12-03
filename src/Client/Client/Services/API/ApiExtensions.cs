namespace Client.Services.API;

using System.Net;
using Auth;
using Refit;

public static class ApiExtensions
{
	public static void AddApi<T>(this IServiceCollection services, Func<IServiceProvider, string> baseAddress) where T : class
	{
		services.AddRefitClient<T>(serviceProvider => new RefitSettings
		        {
			        AuthorizationHeaderValueGetter = async (_, token) =>
			        {
				        var authService = serviceProvider.GetRequiredService<IAuthService>();
				        var tokenResult = await authService.AcquireTokenSilent(token);
				        return tokenResult.IsSuccessful ? tokenResult.Value : string.Empty;
			        }
		        })
		        .ConfigureHttpClient((s,c) =>
		        {
			        c.BaseAddress = new Uri(baseAddress(s));
		        });
	}

	public static async Task<IApiResponse> GetErrorResponse(this Exception e, HttpMethod httpMethod)
	{
		var refitSettings = new RefitSettings();
		var httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
		var apiResponse = new ApiResponse<object>(httpResponse, null, refitSettings,
		                                          await ApiException.Create(new HttpRequestMessage(), httpMethod,
		                                                                    httpResponse, refitSettings, e));
		return apiResponse;
	}

	public static async Task<ApiResponse<T>> GetErrorResponse<T>(this Exception e, HttpMethod httpMethod)
		where T : class
	{
		var refitSettings = new RefitSettings();
		var httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
		var apiResponse = new ApiResponse<T>(httpResponse, null, refitSettings,
		                                     await ApiException.Create(new HttpRequestMessage(), httpMethod,
		                                                               httpResponse, refitSettings, e));
		return apiResponse;
	}
}