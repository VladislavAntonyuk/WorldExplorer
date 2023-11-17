#if MACCATALYST
namespace Client.Services.Auth;

using Microsoft.Extensions.Options;

internal class MacCatalystAuthService(IOptions<AzureB2CConfiguration> options) : IAuthService
{
	private string? currentUserToken;

	public async Task<IOperationResult<string>> SignInInteractively(CancellationToken cancellationToken)
	{
		try
		{
			var loginUrl = GetLoginUrl();
			var authResult = await WebAuthenticator.Default.AuthenticateAsync(
				loginUrl,
				new Uri($"msal{options.Value.ClientId}://auth"));

			currentUserToken = authResult.AccessToken;
			return new OperationResult<string>()
			{
				Value = authResult.AccessToken
			};
		}
		catch (Exception e)
		{
			var operationResult = new OperationResult<string>();
			operationResult.AddError(e.Message);
			return operationResult;
		}
	}

	private Uri GetLoginUrl()
	{
		return new Uri(
			$"https://drawgo.b2clogin.com/drawgo.onmicrosoft.com/oauth2/v2.0/authorize?p={options.Value.SignInPolicy}&client_id={options.Value.ClientId}&nonce=defaultNonce&redirect_uri=msal{options.Value.ClientId}%3A%2F%2Fauth&scope={string.Join("%20", options.Value.Scopes)}&response_type=id_token%20token&prompt=login");
	}

	public async Task<IOperationResult<string>> AcquireTokenSilent(CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
		if (string.IsNullOrEmpty(currentUserToken))
		{
			var operationResult = new OperationResult<string>();
            operationResult.AddError("User not found");
            return operationResult;
		}

		return new OperationResult<string>()
		{
			Value = currentUserToken
		};
	}

	public Task LogoutAsync(CancellationToken cancellationToken)
	{
		currentUserToken = null;
		return Task.CompletedTask;
	}
}
#endif