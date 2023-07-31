namespace WebApp.Services;

using System.Text.Json;
using global::Shared.Models;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

public interface IAiService
{
	Task<List<Place>> GetNearByPlaces(Location location);
}

public class AiService : IAiService
{
	private readonly OpenAIAPI api;
	private readonly ILogger<AiService> logger;

	public AiService(IConfiguration configuration, ILogger<AiService> logger)
	{
		api = new OpenAIAPI(configuration.GetValue<string>("OpenAI:ApiKey"));
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
		}, Model.GPT4);
		if (result.Choices.Count == 0)
		{
			return new List<Place>();
		}

		logger.LogInformation("Received a response from AI: {Response}", result.Choices[0].Message.Content);
		return JsonSerializer.Deserialize<List<Place>>(result.Choices[0].Message.Content, new JsonSerializerOptions
		       {
			       PropertyNameCaseInsensitive = true
		       }) ??
		       new List<Place>();
	}
}