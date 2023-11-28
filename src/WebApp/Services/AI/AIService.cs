namespace WebApp.Services.AI;

using System.Text.Json;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using Shared.Models;

public class AiService : IAiService
{
	private readonly OpenAIAPI api;
	private readonly ILogger<AiService> logger;
	private readonly OpenAiSettings openAiSettings;

	private readonly JsonSerializerOptions serializerOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

public AiService(IOptions<OpenAiSettings> openAiSettings, ILogger<AiService> logger)
	{
		this.openAiSettings = openAiSettings.Value;
		api = new OpenAIAPI(this.openAiSettings.ApiKey);
		this.logger = logger;
	}

	public async Task<List<Place>> GetNearByPlaces(Location location)
	{
		var generalPrompt = $$"""
		                      You are a tour guide with a great knowledge of history. Tell me about 5 places near the following location: Latitude: {{location.Latitude}},Longitude: {{location.Longitude}}.
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
		var result = await api.Chat.CreateChatCompletionAsync(new[]
		{
			new ChatMessage(ChatMessageRole.Assistant, generalPrompt)
		}, new Model(openAiSettings.Model));
		if (result.Choices.Count == 0)
		{
			return [];
		}

		logger.LogInformation("Received a response from AI: {Response}", result.Choices[0].Message.Content);
		return JsonSerializer.Deserialize<List<Place>>(result.Choices[0].Message.Content, serializerOptions) ?? [];
	}
}