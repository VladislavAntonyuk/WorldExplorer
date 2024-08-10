namespace WebApp.Services.AI;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Images;
using OpenAI_API.Models;
using Shared.Extensions;

public class OpenAiProvider(IOptions<AiSettings> aiSettings, ILogger<OpenAiProvider> logger) : IAiProvider
{
	private readonly IOpenAIAPI api = new OpenAIAPI(aiSettings.Value.ApiKey);

	// https://platform.openai.com/docs/models/model-endpoint-compatibility
	private const string GptModel = "gpt-3.5-turbo";

	public async Task<string?> GetResponse(string request)
	{
		var result = await api.Chat.CreateChatCompletionAsync(new ChatRequest
		{
			ResponseFormat = ChatRequest.ResponseFormats.Text,
			Messages = new List<ChatMessage>
			{
				new (ChatMessageRole.System, "You are a tour guide with a great knowledge of history."),
				new (ChatMessageRole.User, request)
			},
			Model = new Model(GptModel)
		}).Safe(new ChatResult { Choices = [] }, e => logger.LogError(e, "Failed to get response"));
		if (result.Choices.Count == 0)
		{
			return null;
		}

		logger.LogInformation("Received a response from AI: {Response}, Duration: {Duration}", result.Choices[0].Message.TextContent, result.ProcessingTime);
		return result.Choices[0].Message.TextContent;
	}

	/// <summary>
	/// Only works with DALL-E-3 and higher (https://platform.openai.com/docs/models/)
	/// </summary>
	public async Task<string?> GetImageResponse(string request)
	{
		try
		{
			var imageResult = await api.ImageGenerations.CreateImageAsync(new ImageGenerationRequest(request, Model.DALLE3, ImageSize._1024, "hd", null, ImageResponseFormat.Url));
			return imageResult.Data.Select(x => x.Url).FirstOrDefault();
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed generating images for {Request}", request);
		}

		return null;
	}
}
