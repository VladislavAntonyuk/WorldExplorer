namespace WorldExplorer.Modules.Places.Infrastructure.AI;

using System.Text.Json;
using Application.Abstractions;
using Common.Infrastructure.Serialization;
using Domain.Places;
using NetTopologySuite.Geometries;
using Location = Application.Abstractions.Location;

public class AiService(IAiProvider aiProvider) : IAiService
{
	public async Task<List<Place>> GetNearByPlaces(Location location)
	{
		var generalPrompt = $$"""
		                      Tell me about 10 places near the following location: Latitude='{{location.Latitude}}', Longitude='{{location.Longitude}}'.
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

		var result = await aiProvider.GetResponse(generalPrompt, AiOutputFormat.Json);
		if (string.IsNullOrWhiteSpace(result))
		{
			return [];
		}

		var response = JsonSerializer.Deserialize<AIResponse>(result, SerializerSettings.Instance);
		return response?.Places
			.Select(x => Place.Create(x.Name,x.Location.ToPoint(), null))
			.ToList() ?? [];
	}

	public Task<string?> GetPlaceDescription(string placeName, Location location)
	{
		var generalPrompt = $"""
		                     Tell me about place named '{placeName}' near the following location: Latitude='{location.Latitude}', Longitude='{location.Longitude}'.
		                     This is a famous place, so you must know a lot about it.
		                     Provide as detailed information as possible. The description for place must contain a lot of text.
		                     The output must contain only the detailed information because I will parse it later. Do not include any information or formatting except description text.
		                     Example:
		                     Dmytro Yavornytsky National Historical Museum of Dnipro is a museum, established in Dnipro (Ukraine) in 1848 by Andriy Fabr, local governor. Its permanent collection consists of 283 thousand objects from ancient Paleolithic implements to display units of World War II. Among its notable objects are the Kurgan stelae, Kernosivsky idol and vast collection of cossack's antiquities.
		                     """;

		return aiProvider.GetResponse(generalPrompt, AiOutputFormat.Text);
	}

	public Task<string?> GenerateImage(string placeName, Location location)
	{
		var prompt =
			$"A photograph of the famous place named '{placeName}' near the following location: Latitude='{location.Latitude}', Longitude='{location.Longitude}'. In output return either url of the image or base64 image. Do not include any another text.";
		return aiProvider.GetImageResponse(prompt);
	}

	private record AiPlaceResponse
	{
		public string Name { get; init; }
		public Location Location { get; init; }
	}

	private class AIResponse
	{
		public AiPlaceResponse[] Places { get; set; } = [];
	}
}