namespace Client.Services.Auth;

using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

internal class AuthService : IAuthService
{
	private readonly IPublicClientApplication authenticationClient;
	private readonly IOptions<AzureB2CConfiguration> azureB2COptions;

	public AuthService(IOptions<AzureB2CConfiguration> azureB2COptions)
	{
		this.azureB2COptions = azureB2COptions;
		authenticationClient = PublicClientApplicationBuilder.Create(azureB2COptions.Value.ClientId)
		                                                     .WithIosKeychainSecurityGroup(
			                                                     azureB2COptions.Value.IosKeychainSecurityGroups)
		                                                     .WithB2CAuthority(azureB2COptions.Value.AuthoritySignIn)
#if WINDOWS
															 .WithRedirectUri("http://localhost")
#else
		                                                     .WithRedirectUri(
			                                                     $"msal{azureB2COptions.Value.ClientId}://auth")
#endif
#if ANDROID
															 .WithParentActivityOrWindow(() => Platform.CurrentActivity)
#endif
		                                                     .Build();
	}

	public async Task<IOperationResult<string>> SignInInteractively(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		try
		{
			var result = await authenticationClient.AcquireTokenInteractive(azureB2COptions.Value.Scopes)
#if WINDOWS
				.WithUseEmbeddedWebView(authenticationClient.IsEmbeddedWebViewAvailable())
#endif
			                                       .ExecuteAsync(cancellationToken);
			return new OperationResult<string>
			{
				Value = result.AccessToken
			};
		}
		catch (MsalClientException ex)
		{
			var operationResult = new OperationResult<string>();
			operationResult.AddError(ex.Message);
			return operationResult;
		}
		catch (OperationCanceledException ex)
		{
			var operationResult = new OperationResult<string>();
			operationResult.AddError(ex.Message);
			return operationResult;
		}
	}

	public async Task<IOperationResult<string>> AcquireTokenSilent(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		try
		{
			var accounts = await authenticationClient.GetAccountsAsync(azureB2COptions.Value.SignInPolicy);
			var firstAccount = accounts.FirstOrDefault();
			if (firstAccount is null)
			{
				var operationResult = new OperationResult<string>();
				operationResult.AddError("User not found");
				return operationResult;
			}

			var result = await authenticationClient.AcquireTokenSilent(azureB2COptions.Value.Scopes, firstAccount)
			                                       .ExecuteAsync(cancellationToken);
			return new OperationResult<string>
			{
				Value = result.AccessToken
			};
		}
		catch (MsalUiRequiredException ex)
		{
			var operationResult = new OperationResult<string>();
			operationResult.AddError(ex.Message);
			return operationResult;
		}
	}

	public async Task LogoutAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var accounts = await authenticationClient.GetAccountsAsync();
		foreach (var account in accounts)
		{
			await authenticationClient.RemoveAsync(account);
		}
	}
}