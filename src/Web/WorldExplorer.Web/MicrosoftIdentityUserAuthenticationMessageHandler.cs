﻿namespace WorldExplorer.Web;

using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

public class MicrosoftIdentityUserAuthenticationMessageHandler(
	ITokenAcquisition tokenAcquisition,
	IOptionsMonitor<MicrosoftIdentityAuthenticationMessageHandlerOptions> namedMessageHandlerOptions,
	string? serviceName = null)
	: MicrosoftIdentityAuthenticationBaseMessageHandler(tokenAcquisition, namedMessageHandlerOptions, serviceName)
{
	private readonly IOptionsMonitor<MicrosoftIdentityAuthenticationMessageHandlerOptions> namedMessageHandlerOptions = namedMessageHandlerOptions;

	/// <inheritdoc />
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		var authResult = await TokenAcquisition
							   .GetAccessTokenForUserAsync(namedMessageHandlerOptions.CurrentValue.GetScopes())
							   .ConfigureAwait(false);

		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult);

		return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
	}
}