namespace WorldExplorer.Modules.Places.Infrastructure.AI;

using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Images;

public class OpenAiProvider(OpenAIClient api, ILogger<OpenAiProvider> logger) : IAiProvider
{
	public async Task<string?> GetResponse(string request)
	{
		var client = api.GetChatClient("gpt-3.5-turbo");

		try
		{
			var result = await client.CompleteChatAsync(
			[
				new SystemChatMessage("You are a tour guide with a great knowledge of history."),
				new UserChatMessage(request)
			], new ChatCompletionOptions
			{
				ResponseFormat = ChatResponseFormat.Text
			});
			if (result.Value.Content.Count == 0)
			{
				return null;
			}

			logger.LogInformation("Received a response from AI: {Response}, Duration: {Duration}", result.Value.Content[0].Text, DateTime.UtcNow - result.Value.CreatedAt.UtcDateTime);
			return result.Value.Content[0].Text;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed generating response for {Request}", request);
			return null;
		}
	}

	/// <summary>
	/// Only works with DALL-E-3 and higher (https://platform.openai.com/docs/models/)
	/// </summary>
	public async Task<string?> GetImageResponse(string request)
	{
		try
		{
			var client = api.GetImageClient("gpt-4");
			var imageResult = await client.GenerateImageAsync(
				request,
				new ImageGenerationOptions
				{
					Quality = GeneratedImageQuality.High,
					ResponseFormat = GeneratedImageFormat.Uri,
					Size = GeneratedImageSize.W1024xH1024,
					Style = GeneratedImageStyle.Natural
				});
			return imageResult.Value.ImageUri.ToString();
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed generating image for {Request}", request);
		}

		return null;
	}
}
