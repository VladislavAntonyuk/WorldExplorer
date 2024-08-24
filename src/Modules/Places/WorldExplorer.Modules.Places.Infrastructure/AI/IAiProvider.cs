namespace WorldExplorer.Modules.Places.Infrastructure.AI;

public interface IAiProvider
{
	Task<string?> GetResponse(string request);

	Task<string?> GetImageResponse(string request);
}