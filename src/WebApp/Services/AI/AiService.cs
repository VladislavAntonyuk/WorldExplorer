namespace WebApp.Services.AI;

using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Images;
using OpenAI_API.Models;
using Shared.Extensions;
using Shared.Models;

public class AiService(IOptions<AiSettings> aiSettings, ILogger<AiService> logger) : IAiService
{
	private readonly JsonSerializerOptions serializerOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	private readonly IOpenAIAPI api = aiSettings.Value.Provider == "Azure"
		? OpenAIAPI.ForAzure("", "", new APIAuthentication(aiSettings.Value.ApiKey))
		: new OpenAIAPI(aiSettings.Value.ApiKey);

	public async Task<List<Place>> GetNearByPlaces(Location location)
	{
		var generalPrompt = $$"""
		                      Tell me about 10 places near the following location: Latitude='{{location.Latitude}}', Longitude='{{location.Longitude}}'.
		                      As they are famous places you must know their coordinates. Provide location as accurately as possible.
		                      Format the output in JSON format with the next properties: name, location (longitude, latitude).
		                      The output must contain only JSON because I will parse it later. Do not include any information or formatting except valid JSON.
		                      Example JSON for one place:
		                          {
		                              "name": "Dmytro Yavornytsky National Historical Museum of Dnipro",
		                              "location": { "latitude": 48.455833330000026, "longitude": 35.06388889000002 }
		                          }
		                      """;

		// https://platform.openai.com/docs/models/model-endpoint-compatibility
		var result = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
		{
			ResponseFormat = ChatRequest.ResponseFormats.JsonObject,
			Messages = new List<ChatMessage>()
			{
				new (ChatMessageRole.System, "You are a tour guide with a great knowledge of history."),
				new (ChatMessageRole.User, generalPrompt)
			},
			Model = new Model(aiSettings.Value.Model)
		}).Safe(new ChatResult{Choices = []});
		if (result.Choices.Count == 0)
		{
			return [];
		}

		logger.LogInformation("Received a response from AI: {Response}, Duration: {Duration}", result.Choices[0].Message.TextContent, result.ProcessingTime);
		var response = JsonSerializer.Deserialize<PlacesAiResponse>(result.Choices[0].Message.TextContent, serializerOptions);
		return response?.Places ?? [];
	}

	public async Task<string?> GetPlaceDescription(string placeName, Location location)
	{
		var generalPrompt = $"""
		                     Tell me about place named '{placeName}' near the following location: Latitude='{location.Latitude}', Longitude='{location.Longitude}'.
		                     This is a famous place, so you must know a lot about it.
		                     Provide as detailed information as possible. The description for place must contain a lot of text (at least 1000 words).
		                     The output must contain only the detailed information because I will parse it later. Do not include any information or formatting except description text.
		                     Example:
		                     Dmytro Yavornytsky National Historical Museum of Dnipro is a museum, established in Dnipro (Ukraine) in 1848 by Andriy Fabr, local governor. Its permanent collection consists of 283 thousand objects from ancient Paleolithic implements to display units of World War II. Among its notable objects are the Kurgan stelae, Kernosivsky idol and vast collection of cossack's antiquities.
		                     """;

		// https://platform.openai.com/docs/models/model-endpoint-compatibility
		var result = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
		{
			ResponseFormat = ChatRequest.ResponseFormats.Text,
			Messages = new List<ChatMessage>()
			{
				new (ChatMessageRole.System, "You are a tour guide with a great knowledge of history."),
				new (ChatMessageRole.User, generalPrompt)
			},
			Model = new Model(aiSettings.Value.Model)
		}).Safe(new ChatResult{Choices = []});
		if (result.Choices.Count == 0)
		{
			return null;
		}

		logger.LogInformation("Received a response from AI: {Response}, Duration: {Duration}", result.Choices[0].Message.TextContent, result.ProcessingTime);
		return result.Choices[0].Message.TextContent;
	}

	public async Task<string?> GenerateImage(string placeName, Location location)
	{
		try
		{
			var prompt = $"A photograph of the famous place named '{placeName}' near the following location: Latitude='{location.Latitude}', Longitude='{location.Longitude}'";
			var imageResult = await api.ImageGenerations.CreateImageAsync(new ImageGenerationRequest(prompt, Model.DALLE3, ImageSize._1024, "hd", null, ImageResponseFormat.Url));
			return imageResult.Data.Select(x => x.Url).First();
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed generating images for {Place}", placeName);
		}

		return null;
	}

	private sealed class PlacesAiResponse
	{
		public List<Place> Places { get; set; } = [];
	}
}