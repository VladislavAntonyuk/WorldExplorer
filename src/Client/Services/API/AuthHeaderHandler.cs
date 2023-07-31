namespace Client.Services.API;

using System.Net.Http.Headers;
using Auth;

internal class AuthHeaderHandler : DelegatingHandler
{
	private readonly IAuthService authService;

	public AuthHeaderHandler(IAuthService authService)
	{
		this.authService = authService;
	}

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