namespace WorldExplorer.Web;

using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

/// <summary>
/// A DelegatingHandler implementation that add an authorization header with a token on behalf of the current user.
/// </summary>
public class MicrosoftIdentityUserAuthenticationMessageHandler : MicrosoftIdentityAuthenticationBaseMessageHandler
{
	private readonly MicrosoftIdentityConsentAndConditionalAccessHandler handler;
	private readonly IOptionsMonitor<MicrosoftIdentityAuthenticationMessageHandlerOptions> namedMessageHandlerOptions;

	/// <summary>
	/// Initializes a new instance of the <see cref="MicrosoftIdentityUserAuthenticationMessageHandler"/> class.
	/// </summary>
	/// <param name="tokenAcquisition">Token acquisition service.</param>
	/// <param name="namedMessageHandlerOptions">Named options provider.</param>
	/// <param name="serviceName">Name of the service describing the downstream web API.</param>
	public MicrosoftIdentityUserAuthenticationMessageHandler(
		ITokenAcquisition tokenAcquisition,
		MicrosoftIdentityConsentAndConditionalAccessHandler handler,
		IOptionsMonitor<MicrosoftIdentityAuthenticationMessageHandlerOptions> namedMessageHandlerOptions,
		string? serviceName = null)
		: base(tokenAcquisition, namedMessageHandlerOptions, serviceName)
	{
		this.handler = handler;
		this.namedMessageHandlerOptions = namedMessageHandlerOptions;
	}

	/// <inheritdoc/>
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var authResult = await TokenAcquisition.GetAccessTokenForUserAsync(namedMessageHandlerOptions.CurrentValue.GetScopes())
											   .ConfigureAwait(false);


		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult);

		try
		{
			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception e)
		{
			handler.HandleException(e);
		}

		return new HttpResponseMessage() { StatusCode = HttpStatusCode.Moved };
	}
}