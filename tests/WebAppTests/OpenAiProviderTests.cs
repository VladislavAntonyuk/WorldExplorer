﻿namespace WebAppTests;

using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using OpenAI;
using WorldExplorer.Modules.Places.Application.Abstractions;
using WorldExplorer.Modules.Places.Infrastructure.AI;

public class OpenAiProviderTests(ITestOutputHelper testOutputHelper) : BaseAiProviderTests(testOutputHelper)
{
	public override Task<IAiService> GetAiService()
	{
		var configuration = new ConfigurationBuilder()
		                    .AddJsonFile("settings.json", false)
		                    .AddJsonFile("settings.Development.json", true)
		                    .Build();
		var client = new OpenAIClient(new ApiKeyCredential($"{configuration["OpenAiKey"]}"), new OpenAIClientOptions
		{
			Endpoint = new Uri($"{configuration["OpenAiEndpoint"]}")
		});
		return Task.FromResult<IAiService>(new AiService(client.GetChatClient("gpt-4o-mini").AsIChatClient(), new NullLogger<AiService>()));
	}
}