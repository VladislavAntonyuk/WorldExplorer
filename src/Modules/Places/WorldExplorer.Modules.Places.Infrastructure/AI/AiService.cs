﻿namespace WorldExplorer.Modules.Places.Infrastructure.AI;

using System.Text.Json;
using Application.Abstractions;
using Common.Infrastructure.Serialization;
using Domain.Places;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Location = Application.Abstractions.Location;

public class AiService(IChatClient client, ILogger<AiService> logger) : IAiService
{
	public async Task<List<Place>> GetNearByPlaces(Location location, CancellationToken cancellationToken)
	{
		var generalPrompt = $$"""
		                      Tell me about all places near the following location: Latitude='{{location.Latitude}}', Longitude='{{location.Longitude}}'.
		                      You must provide at least 10 places, but the more you provide, the better. You must provide places in 10 km zone.
		                      As they are famous places you must know their coordinates. Provide location as accurately as possible.
		                      Format the output in JSON format with the next properties: name, location (longitude, latitude).
		                      The output must contain only JSON because I will parse it later. Do not include any information or formatting except valid JSON.
		                      Example JSON:
		                      {
		                          "places": [
		                              {
		                                  "name": "Dmytro Yavornytsky National Historical Museum of Dnipro",
		                                  "location": { "latitude": 48.455833330000026, "longitude": 35.06388889000002 }
		                              }
		                          ]
		                      }
		                      """;

		var options = new ChatOptions
		{
			ResponseFormat = ChatResponseFormat.Json
		};
		var result = await GetResponse(generalPrompt, options, cancellationToken);
		if (string.IsNullOrWhiteSpace(result))
		{
			return [];
		}

		var response = JsonSerializer.Deserialize<AIResponse>(result, SerializerSettings.Instance);
		return response?.Places.Select(place => Place.Create(place.Name, place.Location.ToPoint())).ToList() ?? [];
	}

	public Task<string?> GetPlaceDescription(string placeName, Location location, CancellationToken cancellationToken)
	{
		var generalPrompt = $"""
		                     Tell me about place named '{placeName}' near the following location: Latitude='{location.Latitude}', Longitude='{location.Longitude}'.
		                     This is a famous place, so you must know a lot about it.
		                     Provide as detailed information as possible. The description for place must contain a lot of text.
		                     The output must contain only the detailed information because I will parse it later. Do not include any information or formatting except description text.
		                     Example:
		                     Dmytro Yavornytsky National Historical Museum of Dnipro is a museum, established in Dnipro (Ukraine) in 1848 by Andriy Fabr, local governor. Its permanent collection consists of 283 thousand objects from ancient Paleolithic implements to display units of World War II. Among its notable objects are the Kurgan stelae, Kernosivsky idol and vast collection of cossack's antiquities.
		                     """;

		return GetResponse(generalPrompt, new ChatOptions { ResponseFormat = ChatResponseFormat.Text }, cancellationToken);
	}

	private async Task<string?> GetResponse(string request,
		ChatOptions options,
		CancellationToken cancellationToken)
	{
		try
		{
			var result = await client.GetResponseAsync(
			[
				new ChatMessage(ChatRole.System, "You are a tour guide with a great knowledge of history."),
				new ChatMessage(ChatRole.User, request)
			], options, cancellationToken);

			return result.Text;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Error has occured during AI request");
			return null;
		}
	}

	private sealed record AiPlaceResponse(string Name, Location Location);

	private sealed class AIResponse
	{
		public AiPlaceResponse[] Places { get; init; } = [];
	}
}