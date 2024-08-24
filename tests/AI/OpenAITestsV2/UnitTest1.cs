namespace OpenAITestsV2;

using System.ClientModel;
using FluentAssertions;
using OpenAI;
using OpenAI.Chat;
using Xunit;
using Xunit.Abstractions;

public class UnitTest1(ITestOutputHelper testOutputHelper)
{
	[Theory]
	[InlineData("https://free.gpt.ge/v1", "API_KEY1", "gpt-3.5-turbo")]
	[InlineData("https://api.chatanywhere.tech/v1", "API_KEY2", "gpt-3.5-turbo")]
	[InlineData("https://free.gpt.ge/v1", "API_KEY1", "gpt-4o-mini")]
	[InlineData("https://api.chatanywhere.tech/v1", "API_KEY2", "gpt-4o-mini")]
	public async Task Test(string url, string key, string model)
	{
		var client = new OpenAIClient(new ApiKeyCredential(key), new OpenAIClientOptions
		{
			Endpoint = new Uri(url)
		});
		var chatClient = client.GetChatClient(model);
		var result = await chatClient.CompleteChatAsync(new UserChatMessage("hello"));
		var content = result.Value.Content[0].ToString();
		testOutputHelper.WriteLine(content);
		content.Should().NotBeEmpty();
	}
}