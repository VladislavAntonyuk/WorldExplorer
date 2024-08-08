namespace WorldExplorer.Modules.Users.Application.Abstractions.Identity;

public interface IGraphClientService
{
	Task<AzureUser?> GetUser(string providerId, CancellationToken cancellationToken);
	Task DeleteAsync(string providerId, CancellationToken cancellationToken);
}