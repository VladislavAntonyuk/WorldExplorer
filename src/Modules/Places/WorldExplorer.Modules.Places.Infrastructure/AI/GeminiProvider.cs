namespace WebApp.Services.AI;

using GenerativeAI.Models;
using GenerativeAI.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Extensions;

public class GeminiProvider(HttpClient httpClient, IOptions<AiSettings> aiOptions, ILogger<GeminiProvider> logger) : IAiProvider
{
	public async Task<string?> GetResponse(string request)
	{
		var result = await new GenerativeModel(aiOptions.Value.ApiKey, client: httpClient).GenerateContentAsync([
			new Part {Text = "You are a tour guide with a great knowledge of history."},
			new Part {Text = request},
		]).Safe(new EnhancedGenerateContentResponse(), e => logger.LogError(e, "Failed to get response"));
		var response = result.Text();
		if (string.IsNullOrEmpty(response))
		{
			return null;
		}

		logger.LogInformation("Received a response from AI: {Response}", response);
		return response;
	}

	public Task<string?> GetImageResponse(string request)
	{
		return Task.FromResult<string?>(null);
	}
}