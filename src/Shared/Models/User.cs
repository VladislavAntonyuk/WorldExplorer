namespace Shared.Models;

using Client.Models;

public class User
{
	public required string Id { get; set; }

	public ICollection<Visit> Visits { get; set; } = new List<Visit>();
	public List<UserActivity> Activities { get; set; } = new();
	public required string Name { get; set; }
	public required string Email { get; set; }
}