namespace WebAppTests;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WebApp.Services.AI;
using Xunit.Abstractions;

public class GeminiProviderTests(ITestOutputHelper testOutputHelper) : BaseAiProviderTests(testOutputHelper)
{
	public override IAiService GetAiService()
	{
		var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Add("x-goog-api-client", "genai-swift/0.4.8");
		httpClient.DefaultRequestHeaders.Add("User-Agent", "AIChat/1 CFNetwork/1410.1 Darwin/22.6.0");
		return new AiService(new GeminiProvider(httpClient, Options.Create(new AiSettings
		{
			ApiKey = "API-key",
			Provider = "Gemini"
		}), NullLogger<GeminiProvider>.Instance));
	}
}