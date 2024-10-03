namespace WebAppTests;

using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Ollama;
using WorldExplorer.Modules.Places.Application.Abstractions;
using WorldExplorer.Modules.Places.Infrastructure.AI;
using Xunit.Abstractions;

public class OllamaProviderTests(ITestOutputHelper testOutputHelper) : BaseAiProviderTests(testOutputHelper)
{
	public override async Task<IAiService> GetAiService()
	{
		var container = new ContainerBuilder()
		                .WithImage("ollama/ollama:0.3.8")
		                .WithPortBinding(11434, true)
		                .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(11434)))
		                .WithVolumeMount(new VolumeBuilder().WithName("ollama").WithReuse(true).Build(), "/root/.ollama")
		                .WithReuse(true)
		                .Build();

		await container.StartAsync()
		               .ConfigureAwait(false);

		var baseUri = new UriBuilder(Uri.UriSchemeHttp, container.Hostname, container.GetMappedPublicPort(11434), "api").Uri;
		var ollamaApiClient = new OllamaApiClient(null, baseUri);

		var model = await ollamaApiClient.Models.ListRunningModelsAsync();
		if (model.Models?.Select(x => x.Model).Contains("llama3") != true)
		{
			await ollamaApiClient.Models.PullModelAsync("llama3:latest");
		}

		return new AiService(new OllamaProvider(ollamaApiClient, NullLogger<OllamaProvider>.Instance));
	}
}