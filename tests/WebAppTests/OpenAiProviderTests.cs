namespace WebAppTests;

using System.ClientModel;
using Microsoft.Extensions.Logging.Abstractions;
using OpenAI;
using WorldExplorer.Modules.Places.Application.Abstractions;
using WorldExplorer.Modules.Places.Infrastructure.AI;
using Xunit.Abstractions;

public class OpenAiProviderTests(ITestOutputHelper testOutputHelper) : BaseAiProviderTests(testOutputHelper)
{
	public override IAiService GetAiService()
	{
		return new AiService(new OpenAiProvider(new OpenAIClient(new ApiKeyCredential("API-Key"),
		                                                         new OpenAIClientOptions
		                                                         {
			                                                         Endpoint = new Uri("https://openai.com")
		                                                         }), NullLogger<OpenAiProvider>.Instance));
	}
}