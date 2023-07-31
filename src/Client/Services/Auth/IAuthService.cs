namespace Client.Services.Auth;

public interface IAuthService
{
	Task<IOperationResult<string>> SignInInteractively(CancellationToken cancellationToken);
	Task<IOperationResult<string>> AcquireTokenSilent(CancellationToken cancellationToken);
	Task LogoutAsync(CancellationToken cancellationToken);
}