namespace WorldExplorer.Modules.Places.Infrastructure.Image;

using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.Extensions.Configuration;

public class ImageSearchService
	(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IImageSearchService
{
	private readonly string? apiKey = configuration.GetValue<string>("GoogleSearch:ApiKey");

	public async Task<List<string>> GetPlaceImages(string placeName, CancellationToken cancellationToken)
	{
		try
		{
			using var httpClient = httpClientFactory.CreateClient("GoogleImages");
			var response = await httpClient.GetFromJsonAsync<GoogleImagesResponse>(
				$"search.json?engine=google_images&q={HttpUtility.UrlEncode(placeName)}&api_key={apiKey}",
				cancellationToken);
			return response?.ImagesResults.Select(x => x.Url).Where(x => x.StartsWith("https://")).ToList() ??
				   [];
		}
		catch
		{
			return [];
		}
	}
}

public class GoogleImagesResponse
{
	[JsonPropertyName("images_results")]
	public List<GoogleImagesResult> ImagesResults { get; set; } = [];
}

public class GoogleImagesResult
{
	[JsonPropertyName("original")]
	public required string Url { get; set; }
}