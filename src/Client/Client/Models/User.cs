namespace Client.Models;

public class User
{
	public required Guid Id { get; set; }

	public ICollection<Visit> Visits { get; set; } = new List<Visit>();
	public List<UserActivity> Activities { get; set; } = [];
	public required string Name { get; set; }
	public required string Email { get; set; }
	public UserSettings Settings { get; set; } = new();
}