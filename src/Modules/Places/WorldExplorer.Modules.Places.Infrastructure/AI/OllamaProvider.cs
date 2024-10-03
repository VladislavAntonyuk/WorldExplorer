namespace WorldExplorer.Modules.Places.Infrastructure.AI;

using Microsoft.Extensions.Logging;
using Ollama;

public class OllamaProvider(IOllamaApiClient api, ILogger<OllamaProvider> logger) : IAiProvider
{
	public async Task<string?> GetResponse(string request)
	{
		try
		{
			var result = await api.Chat.GenerateChatCompletionAsync(new GenerateChatCompletionRequest()
			{
				Model = "llama3.2",
				Messages = [
					new Message(MessageRole.System, "You are a tour guide with a great knowledge of history."),
					new Message(MessageRole.User, request)
				],
				Format = ResponseFormat.Json
			});
			
			logger.LogInformation("Received a response from AI: {Response}, Duration: {Duration}",
								  result.Message.Content, DateTime.UtcNow - result.CreatedAt);
			 return result.Message.Content;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed generating response for {Request}", request);
			return null;
		}
	}

	public async Task<string?> GetImageResponse(string request)
	{
		try
		{
			var result = await api.Chat.GenerateChatCompletionAsync(new GenerateChatCompletionRequest()
			{
				Model = "llava",
				Messages = [new Message(MessageRole.User, request)]
			});
			
			return result.Message.Images?.FirstOrDefault() ?? result.Message.Content;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed generating image for {Request}", request);
		}

		return null;
	}
}