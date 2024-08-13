namespace OpenAITestsV1;

using System.ClientModel;
using OpenAI;
using OpenAI.Chat;
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
		var client = new OpenAIClient(new ApiKeyCredential("API-KEY"),
		                              new OpenAIClientOptions()
		                              {
										  Endpoint = new Uri("https://free.gpt.ge/v1")
									  });
		var chatClient = client.GetChatClient("gpt-3.5-turbo");
		var result = await chatClient.CompleteChatAsync(new UserChatMessage("hello"));
		testOutputHelper.WriteLine(result.Value.Content[0].ToString());
	}
}