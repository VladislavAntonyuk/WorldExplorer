namespace Client.Services.Auth;

internal class MockAuthService : IAuthService
{
	public async Task<IOperationResult<string>> SignInInteractively(CancellationToken cancellationToken)
	{
		await Task.Delay(1, cancellationToken);
		return new OperationResult<string>
		{
			Value = "token"
		};
	}

	public async Task<IOperationResult<string>> AcquireTokenSilent(CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
		return new OperationResult<string>
		{
			Value = ""
		};
	}

	public Task LogoutAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}