namespace OpenAITestsV1;

using FluentAssertions;
using Microsoft.Extensions.Configuration;
using OpenAI_API;
using OpenAI_API.Chat;
using Xunit;
using Xunit.Abstractions;

public class UnitTest1(ITestOutputHelper testOutputHelper)
{
	[Theory]
	[InlineData("https://free.gpt.ge/", "API_KEY1", "gpt-3.5-turbo")]
	[InlineData("https://api.chatanywhere.tech/", "API_KEY2", "gpt-3.5-turbo")]
	[InlineData("https://free.gpt.ge/", "API_KEY1", "gpt-4o-mini")]
	[InlineData("https://api.chatanywhere.tech/", "API_KEY2", "gpt-4o-mini")]
	public async Task Test(string url, string key, string model)
	{
		var configuration = new ConfigurationBuilder()
		                    .AddJsonFile("settings.json", false)
		                    .AddJsonFile("settings.Development.json", true)
		                    .Build();

		var client = new OpenAIAPI(new APIAuthentication(configuration[key]))
		{
			ApiUrlFormat = $"{url}/{{0}}/{{1}}",
			ApiVersion = "v1"
		};
		client.Chat.DefaultChatRequestArgs.Model = model;
		var result = await client.Chat.CreateChatCompletionAsync(new ChatMessage(ChatMessageRole.User, "hello"));
		var content = result.Choices[0].Message.TextContent;
		testOutputHelper.WriteLine(content);
		content.Should().NotBeEmpty();
	}
}