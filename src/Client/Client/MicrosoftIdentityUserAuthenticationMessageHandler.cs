namespace Client;

using System.Net;
using System.Net.Http.Headers;
using Services.Auth;

public class MicrosoftIdentityUserAuthenticationMessageHandler(IAuthService authService) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var authResult = await authService.AcquireTokenSilent(cancellationToken).ConfigureAwait(false);
		if (authResult.IsSuccessful)
		{
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.Value);
			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
		}

		return new HttpResponseMessage(HttpStatusCode.Unauthorized);
	}
}