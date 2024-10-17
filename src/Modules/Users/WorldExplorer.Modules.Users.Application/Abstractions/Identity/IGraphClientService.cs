namespace WorldExplorer.Modules.Users.Application.Abstractions.Identity;

public interface IGraphClientService
{
	Task<AzureUser?> GetUser(Guid providerId, CancellationToken cancellationToken);
	Task DeleteAsync(Guid providerId, CancellationToken cancellationToken);
}