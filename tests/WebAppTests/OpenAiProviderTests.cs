namespace WebAppTests;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WebApp.Services.AI;
using Xunit.Abstractions;

public class OpenAiProviderTests(ITestOutputHelper testOutputHelper) : BaseAiProviderTests(testOutputHelper)
{
	public override IAiService GetAiService()
	{
		return new AiService(new OpenAiProvider(Options.Create(new AiSettings
		{
			ApiKey = "API-key",
			Provider = "OpenAI"
		}), NullLogger<OpenAiProvider>.Instance));
	}
}