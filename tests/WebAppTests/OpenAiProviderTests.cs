namespace WebAppTests;

using System.ClientModel;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OpenAI;
using WebApp.Services.AI;
using Xunit.Abstractions;

public class OpenAiProviderTests(ITestOutputHelper testOutputHelper) : BaseAiProviderTests(testOutputHelper)
{
	public override IAiService GetAiService()
	{
		return new AiService(new OpenAiProvider(new OpenAIClient(new ApiKeyCredential("API-key")), NullLogger<OpenAiProvider>.Instance));
	}
}