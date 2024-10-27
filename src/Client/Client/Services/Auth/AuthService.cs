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
															 .WithB2CAuthority(azureB2COptions.Value.AuthoritySignIn)
															 .WithHttpClientFactory(new AuthHttpClientFactory())
#if WINDOWS
															 .WithRedirectUri("http://localhost")
#else
															 .WithRedirectUri($"msal{azureB2COptions.Value.ClientId}://auth")
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
#if WINDOWS
			await AttachTokenCache();
#endif
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
		catch (Exception ex)
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
#if WINDOWS
			await AttachTokenCache();
#endif
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
		catch (Exception ex)
		{
			var operationResult = new OperationResult<string>();
			operationResult.AddError(ex.Message);
			return operationResult;
		}
	}

	public async Task LogoutAsync(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
#if WINDOWS
		await AttachTokenCache();
#endif
		var accounts = await authenticationClient.GetAccountsAsync();
		foreach (var account in accounts)
		{
			await authenticationClient.RemoveAsync(account);
		}
	}

#if WINDOWS
	private async Task AttachTokenCache()
	{
		var cacheDir = Path.Join(Path.GetTempPath(), "WorldExplorer");
		// Cache configuration and hook-up to public application. Refer to https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache#configuring-the-token-cache
		var storageProperties = new Microsoft.Identity.Client.Extensions.Msal.StorageCreationPropertiesBuilder("WorldExplorerCache.cache", cacheDir).Build();
		var msalCacheHelper = await Microsoft.Identity.Client.Extensions.Msal.MsalCacheHelper.CreateAsync(storageProperties);
		msalCacheHelper.RegisterCache(authenticationClient.UserTokenCache);
	}
#endif
}