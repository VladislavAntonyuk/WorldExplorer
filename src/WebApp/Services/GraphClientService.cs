namespace WebApp.Services;

using Microsoft.Graph.Beta;

public interface IGraphClientService
{
	Task DeleteUser(string providerId, CancellationToken cancellationToken);
}

public class GraphClientService : IGraphClientService
{
	private readonly GraphServiceClient graphClient;

	public GraphClientService(GraphServiceClient graphClient)
	{
		this.graphClient = graphClient;
	}

	public Task DeleteUser(string providerId, CancellationToken cancellationToken)
	{
		return graphClient.Users[providerId].DeleteAsync(cancellationToken: cancellationToken);
	}
}