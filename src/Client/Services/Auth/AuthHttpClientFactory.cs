namespace Client.Services.Auth;

using Microsoft.Identity.Client;

internal class AuthHttpClientFactory : IMsalHttpClientFactory
{
	public HttpClient GetHttpClient()
	{
		return new HttpClient
		{
			Timeout = TimeSpan.FromSeconds(10)
		};
	}
}