namespace WebApp.Services;

using System.Text.Json.Serialization;
using System.Web;

public class ImageSearchService : IImageSearchService
{
	private readonly string apiKey;
	private readonly IHttpClientFactory httpClientFactory;

	public ImageSearchService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
	{
		this.httpClientFactory = httpClientFactory;
		apiKey = configuration.GetValue<string>("GoogleSearch:ApiKey") ??
				 throw new NullReferenceException("GoogleSearch ApiKey is null");
	}

	public async Task<List<string>> GetPlaceImages(string placeName, CancellationToken cancellationToken)
	{
		try
		{
			using var httpClient = httpClientFactory.CreateClient("GoogleImages");
			var response = await httpClient.GetFromJsonAsync<GoogleImagesResponse>(
				$"search.json?engine=google_images&q={HttpUtility.UrlEncode(placeName)}&api_key={apiKey}",
				cancellationToken);
			return response?.ImagesResults.Select(x => x.Url).Where(x => x.StartsWith("https://")).ToList() ??
				   new List<string>();
		}
		catch (Exception)
		{
			return new List<string>();
		}
	}
}

public class GoogleImagesResponse
{
	[JsonPropertyName("images_results")]
	public List<GoogleImagesResult> ImagesResults { get; set; } = new();
}

public class GoogleImagesResult
{
	[JsonPropertyName("original")]
	public required string Url { get; set; }
}