namespace WebApp.Apis.V1.Controllers.Models;

using System.Text.Json.Serialization;

public class RequestConnector
{
	[JsonPropertyName("step")]
	public string Step { get; set; } = string.Empty;

	[JsonPropertyName("client_id")]
	public string ClientId { get; set; } = string.Empty;

	[JsonPropertyName("ui_locales")]
	public string UiLocales { get; set; } = string.Empty;

	[JsonPropertyName("email")]
	public string Email { get; set; } = string.Empty;

	[JsonPropertyName("objectId")]
	public string ObjectId { get; set; } = string.Empty;

	[JsonPropertyName("surname")]
	public string Surname { get; set; } = string.Empty;

	[JsonPropertyName("displayName")]
	public string DisplayName { get; set; } = string.Empty;

	[JsonPropertyName("givenName")]
	public string GivenName { get; set; } = string.Empty;
}