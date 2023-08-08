namespace WebApp.Services;

public interface IGraphClientService
{
	Task DeleteUser(string providerId, CancellationToken cancellationToken);
}