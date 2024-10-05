namespace WorldExplorer.Modules.Places.Infrastructure.AI;

public interface IAiProvider
{
	Task<string?> GetResponse(string request, AiOutputFormat outputFormat);

	Task<string?> GetImageResponse(string request);
}

public enum AiOutputFormat
{
	Text,
	Json
}