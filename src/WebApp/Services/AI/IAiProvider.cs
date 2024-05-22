namespace WebApp.Services.AI;

public interface IAiProvider
{
	Task<string?> GetResponse(string request);

	Task<string?> GetImageResponse(string request);
}