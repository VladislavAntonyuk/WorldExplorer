namespace OpenAITestsV1;

using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using Xunit;
using Xunit.Abstractions;

public class UnitTest1
{
	private readonly ITestOutputHelper testOutputHelper;

	public UnitTest1(ITestOutputHelper testOutputHelper)
	{
		this.testOutputHelper = testOutputHelper;
	}

	[Fact]
	public async Task Test1()
	{
		var client = new OpenAI_API.OpenAIAPI(new APIAuthentication("API-KEY"));
		client.ApiUrlFormat = "https://free.gpt.ge/{0}/{1}";
		client.ApiVersion = "v1";
		client.Chat.DefaultChatRequestArgs.Model = Model.ChatGPTTurbo;
		var result = await client.Chat.CreateChatCompletionAsync(new ChatMessage(ChatMessageRole.User, "hello"));
		testOutputHelper.WriteLine(result.Choices[0].Message.TextContent);
	}
}