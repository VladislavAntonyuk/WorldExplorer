namespace WebApp.Services.AI;

using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using Place;
using Shared.Models;

public class AiService(IOptions<AiSettings> aiSettings, ILogger<AiService> logger) : IAiService
{
	private readonly JsonSerializerOptions serializerOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	public async Task<List<Place>> GetNearByPlaces(Location location)
	{
		var generalPrompt = $$"""
		                      You are a tour guide with a great knowledge of history. Tell me about 5 places near the following location: Latitude: {{location.Latitude}},Longitude: {{location.Longitude}}.
		                      Places must be located within {{DistanceConstants.NearbyDistance}} meters from the location.
		                      Format the output in json format with the next properties: name, description, location (longitude, latitude).
		                      As they are famous places you must know there coordinates. Provide as most detailed information in description as possible. Description must contain a lot of text (at least 500 words).
		                      The output must contain only json, because I will parse it later. Do not include any information or formatting except valid json.
		                      Example:
		                      [
		                          {
		                              "name": "Menorah Centre",
		                              "description": "One of the world's largest Jewish centers which includes a synagogue, museum, and art center. It was created many years ago.",
		                              "location": { "latitude": 48.457186971565974, "longitude": 35.03002213777964 }
		                          }
		                      ]
		                      """;
		// https://platform.openai.com/docs/models/model-endpoint-compatibility
		var api = aiSettings.Value.Provider == "Azure"
			? OpenAIAPI.ForAzure("", "", new APIAuthentication(aiSettings.Value.ApiKey))
			: new OpenAIAPI(aiSettings.Value.ApiKey);
		var result = await api.Chat.CreateChatCompletionAsync(new[]
		{
			new ChatMessage(ChatMessageRole.Assistant, generalPrompt)
		}, new Model(aiSettings.Value.Model));
		if (result.Choices.Count == 0)
		{
			return [];
		}

		logger.LogInformation("Received a response from AI: {Response}, Duration: {Duration}", result.Choices[0].Message.Content, result.ProcessingTime);
		return JsonSerializer.Deserialize<List<Place>>(result.Choices[0].Message.Content, serializerOptions) ?? [];
	}
}