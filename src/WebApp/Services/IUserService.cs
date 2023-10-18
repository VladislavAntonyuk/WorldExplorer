namespace WebApp.Services;

using Shared.Models;

public interface IUserService
{
	Task<List<User>> GetUsers(CancellationToken cancellationToken);
	Task<User?> GetUser(string providerId, CancellationToken cancellationToken);
	Task DeleteUser(string providerId, CancellationToken cancellationToken);
}