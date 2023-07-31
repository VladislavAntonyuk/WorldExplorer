namespace WebApp.Services;

using System.Text.Encodings.Web;
using Newtonsoft.Json.Linq;

public interface IImageSearchService
{
	Task<List<string>> GetPlaceImages(string placeName, CancellationToken cancellationToken);
}

public class ImageSearchService : IImageSearchService
{
	private readonly IHttpClientFactory httpClientFactory;
	private readonly IConfiguration configuration;

	public ImageSearchService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
	{
		this.httpClientFactory = httpClientFactory;
		this.configuration = configuration;
	}

	public async Task<List<string>> GetPlaceImages(string placeName, CancellationToken cancellationToken)
	{
		try
		{
			using var httpClient = httpClientFactory.CreateClient("GoogleImages");
			var result = await httpClient.GetStringAsync(
				$"search.json?engine=google_images&q={UrlEncoder.Default.Encode(placeName)}&api_key=${configuration.GetValue<string>("GoogleSearch:ApiKey")}",
				cancellationToken);
			var data = JObject.Parse(result);
			var results = (JArray)data["images_results"];

			return results.Select(x => x["original"].ToString()).Where(x => x.StartsWith("https://")).ToList();
		}
		catch (Exception ex)
		{
			return new List<string>();
		}
	}
}