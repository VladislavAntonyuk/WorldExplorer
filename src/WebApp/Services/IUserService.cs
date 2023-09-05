namespace WebApp.Services;

using global::Shared.Models;

public interface IUserService
{
	Task<User?> GetUser(string providerId, CancellationToken cancellationToken);
	Task DeleteUser(string providerId, CancellationToken cancellationToken);
}