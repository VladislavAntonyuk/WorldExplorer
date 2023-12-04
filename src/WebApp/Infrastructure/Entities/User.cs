namespace WebApp.Infrastructure.Entities;

public class User
{
	public required string Id { get; set; }

	public ICollection<Visit> Visits { get; set; } = new List<Visit>();

	public UserSettings Settings { get; set; } = new();
}
