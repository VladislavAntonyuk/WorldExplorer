namespace WebApp.Services;

using System.Web;
using Newtonsoft.Json.Linq;

public interface IImageSearchService
{
	Task<List<string>> GetPlaceImages(string placeName, CancellationToken cancellationToken);
}

public class ImageSearchService : IImageSearchService
{
	private readonly IHttpClientFactory httpClientFactory;
	private readonly string apiKey;

	public ImageSearchService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
	{
		this.httpClientFactory = httpClientFactory;
		apiKey = configuration.GetValue<string>("GoogleSearch:ApiKey") ?? throw new ArgumentNullException(nameof(apiKey));
	}

	public async Task<List<string>> GetPlaceImages(string placeName, CancellationToken cancellationToken)
	{
		try
		{
			using var httpClient = httpClientFactory.CreateClient("GoogleImages");
			var result = await httpClient.GetStringAsync($"search.json?engine=google_images&q={HttpUtility.UrlEncode(placeName)}&api_key={apiKey}", cancellationToken);
			var data = JObject.Parse(result);
			var results = (JArray)data["images_results"];

			return results.Select(x => x["original"].ToString().Trim()).Where(x => x.StartsWith("https://")).ToList();
		}
		catch (Exception ex)
		{
			return new List<string>();
		}
	}
}