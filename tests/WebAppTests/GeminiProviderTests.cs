namespace WebAppTests;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WorldExplorer.Modules.Places.Application.Abstractions;
using WorldExplorer.Modules.Places.Infrastructure.AI;
using Xunit.Abstractions;

public class GeminiProviderTests(ITestOutputHelper testOutputHelper) : BaseAiProviderTests(testOutputHelper)
{
	public override Task<IAiService> GetAiService()
	{
		var configuration = new ConfigurationBuilder()
		                    .AddJsonFile("settings.json", false)
		                    .AddJsonFile("settings.Development.json", true)
		                    .Build();

		var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Add("x-goog-api-client", "genai-swift/0.4.8");
		httpClient.DefaultRequestHeaders.Add("User-Agent", "AIChat/1 CFNetwork/1410.1 Darwin/22.6.0");
		return Task.FromResult<IAiService>(new AiService(new GeminiProvider(httpClient, Options.Create(new GeminiAiSettings
		{
			ApiKey = configuration["GeminiKey"]
		}), NullLogger<GeminiProvider>.Instance)));
	}
}