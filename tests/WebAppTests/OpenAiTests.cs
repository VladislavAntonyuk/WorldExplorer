namespace WebAppTests;

using System.ClientModel;
using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using Xunit.Abstractions;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

public class OpenAiTests(ITestOutputHelper testOutputHelper)
{
	[Theory]
	[InlineData("https://free.gpt.ge/v1", "API_KEY1", "gpt-3.5-turbo")]
	[InlineData("https://api.chatanywhere.tech/v1", "API_KEY2", "gpt-3.5-turbo")]
	[InlineData("https://free.gpt.ge/v1", "API_KEY1", "gpt-4o-mini")]
	[InlineData("https://api.chatanywhere.tech/v1", "API_KEY2", "gpt-4o-mini")]
	public async Task Test(string url, string key, string model)
	{
		var configuration = new ConfigurationBuilder()
		                    .AddJsonFile("settings.json", false)
		                    .AddJsonFile("settings.Development.json", true)
		                    .Build();

		var client = new OpenAIChatClient(new OpenAIClient(new ApiKeyCredential($"{configuration[key]}"), new OpenAIClientOptions
		{
			Endpoint = new Uri(url)
		}), model);
		var result = await client.CompleteAsync([new ChatMessage(ChatRole.User, "hello")]);
		var content = result.Message.Text;
		testOutputHelper.WriteLine(content);
		content.Should().NotBeEmpty();
	}
}