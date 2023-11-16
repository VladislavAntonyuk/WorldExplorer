#if MACCATALYST
namespace Client.Services.Auth;

using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.Options;

public class WebBrowserAuthenticator : IBrowser
{
	public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
	{
		try
		{
			WebAuthenticatorResult result = await WebAuthenticator.Default.AuthenticateAsync(
				new Uri(options.StartUrl),
				new Uri(options.EndUrl));

			var url = new RequestUrl(options.EndUrl)
				.Create(new Parameters(result.Properties));

			// Workaround for Facebook issue
			if (url.EndsWith("%23_%3D_"))
			{
				url = url.Substring(0, url.LastIndexOf("%23_%3D_", StringComparison.Ordinal));
			}

			return new BrowserResult
			{
				Response = url,
				ResultType = BrowserResultType.Success
			};
		}
		catch (TaskCanceledException)
		{
			return new BrowserResult
			{
				ResultType = BrowserResultType.UserCancel,
				ErrorDescription = "Login canceled by the user."
			};
		}
	}
}

internal class MacCatalystAuthService : IAuthService
{
	private readonly OidcClient oidcClient;

	public MacCatalystAuthService(IOptions<AzureB2CConfiguration> options)
	{
		var configuration = options.Value;
		oidcClient = new OidcClient(new OidcClientOptions
		{
			Authority = configuration.AuthoritySignIn,
			ClientId = configuration.ClientId,
			Scope = string.Join(' ', configuration.Scopes),
			RedirectUri = $"msal{configuration.ClientId}://auth",
			Browser = new WebBrowserAuthenticator()
		});
	}

	public async Task<IOperationResult<string>> SignInInteractively(CancellationToken cancellationToken)
	{
		var loginResult = await oidcClient.LoginAsync(cancellationToken: cancellationToken);
		if (!loginResult.IsError)
		{
			return new OperationResult<string>()
			{
				Value = loginResult.AccessToken
			};
		}

		var operationResult = new OperationResult<string>();
		operationResult.AddError(loginResult.Error ?? loginResult.ErrorDescription);
		return operationResult;
	}

	public Task<IOperationResult<string>> AcquireTokenSilent(CancellationToken cancellationToken)
	{
		return SignInInteractively(cancellationToken);
	}

	public Task LogoutAsync(CancellationToken cancellationToken)
	{
		return oidcClient.LogoutAsync(cancellationToken: cancellationToken);
	}
}
#endif