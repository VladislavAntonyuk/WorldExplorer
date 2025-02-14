﻿namespace WorldExplorer.Modules.Places.Infrastructure.Image;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Application.Abstractions;
using Microsoft.Extensions.Options;

public class ImageSearchService(IHttpClientFactory httpClientFactory, IOptions<ImageSearchSettings> imageSearch)
	: IImageSearchService
{
	private readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
	};

	public async Task<List<string>> GetPlaceImages(string placeName, CancellationToken cancellationToken)
	{
		try
		{
			using var httpClient = httpClientFactory.CreateClient("ImageSearch");
			httpClient.BaseAddress = new Uri(imageSearch.Value.BaseAddress);
			var response = await httpClient.GetFromJsonAsync<GoogleImagesResponse>(
				$"search.json?engine=google_images&q={HttpUtility.UrlEncode(placeName)}&api_key={imageSearch.Value.ApiKey}",
				jsonSerializerOptions,
				cancellationToken);
			return response?.ImagesResults.Select(x => x.Url).Where(x => x.StartsWith("https://")).ToList() ?? [];
		}
		catch
		{
			return [];
		}
	}

	public async Task<string?> GenerateImage(string placeName, Location location, CancellationToken cancellationToken)
	{
		var prompt = $"A photograph of the famous place named '{placeName}' near the following location: Latitude='{location.Latitude}', Longitude='{location.Longitude}'";
		using var httpClient = httpClientFactory.CreateClient();
		httpClient.Timeout = TimeSpan.FromMinutes(5);
		var response = await httpClient.GetByteArrayAsync(
			$"https://image.pollinations.ai/prompt/{HttpUtility.UrlEncode(prompt)}?nologo=true&private=true",
			cancellationToken);
		return Convert.ToBase64String(response);
	}
}

public class GoogleImagesResponse
{
	public List<GoogleImagesResult> ImagesResults { get; set; } = [];
}

public class GoogleImagesResult
{
	[JsonPropertyName("original")]
	public required string Url { get; set; }
}