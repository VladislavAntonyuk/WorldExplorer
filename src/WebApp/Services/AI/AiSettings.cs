namespace WebApp.Services.AI;

public class AiSettings
{
	public required string ApiKey { get; set; }

	public required string Model { get; set; }

	public required string Provider { get; set; }
}