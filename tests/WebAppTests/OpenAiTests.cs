﻿namespace WebAppTests;

using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using Shouldly;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

public class OpenAiTests(ITestOutputHelper testOutputHelper)
{
	[Theory]
	[InlineData("https://api.chatanywhere.tech/v1", "API_KEY2", "gpt-3.5-turbo")]
	[InlineData("https://api.chatanywhere.tech/v1", "API_KEY2", "gpt-4o-mini")]
	public async Task Test(string url, string key, string model)
	{
		var configuration = new ConfigurationBuilder()
		                    .AddJsonFile("settings.json", false)
		                    .AddJsonFile("settings.Development.json", true)
		                    .Build();
		
		var client = new OpenAIClient(new ApiKeyCredential($"{configuration[key]}"), new OpenAIClientOptions
		{
			Endpoint = new Uri(url)
		}).GetChatClient(model).AsIChatClient();
		var result = await client.GetResponseAsync([new ChatMessage(ChatRole.User, "hello")], cancellationToken: TestContext.Current.CancellationToken);
		var content = result.Text;
		testOutputHelper.WriteLine(content);
		content.ShouldNotBeEmpty();
	}
}