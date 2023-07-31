namespace WebApp.Apis.V1.Controllers.Models;

using System.Text.Json.Serialization;

public class ResponseContent
{
	private const string ApiVersion = "1.0.0";

	public ResponseContent() : this("Continue")
	{
	}

	public ResponseContent(string action, string? userMessage = null)
	{
		Version = ApiVersion;
		Action = action;
		UserMessage = userMessage;
		if (action == "ValidationError")
		{
			Status = "400";
		}
	}

	[JsonPropertyName("version")]
	public string Version { get; }

	[JsonPropertyName("action")]
	public string Action { get; set; }

	[JsonPropertyName("userMessage")]
	public string? UserMessage { get; set; }

	[JsonPropertyName("status")]
	public string? Status { get; set; }

	[JsonPropertyName("extension_Groups")]
	public string Groups { get; set; } = string.Empty;

	[JsonPropertyName("extension_Language")]
	public string Language { get; set; } = string.Empty;
}