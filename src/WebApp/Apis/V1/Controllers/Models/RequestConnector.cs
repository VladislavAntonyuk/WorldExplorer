namespace WebApp.Apis.V1.Controllers.Models;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class RequestConnector
{
	[JsonPropertyName("client_id")]
	[Required]
	public required string ClientId { get; set; }

	[JsonPropertyName("objectId")]
	[Required]
	public required string ObjectId { get; set; }
}