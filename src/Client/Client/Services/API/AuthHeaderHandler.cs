namespace Client.Services.API;

using System.Net.Http.Headers;
using Auth;

internal class AuthHeaderHandler(IAuthService authService) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		var getTokenResult = await authService.AcquireTokenSilent(cancellationToken);
		if (getTokenResult.IsSuccessful)
		{
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", getTokenResult.Value);
		}

		return await base.SendAsync(request, cancellationToken);
	}
}