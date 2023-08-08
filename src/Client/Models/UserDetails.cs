namespace Client.Models;

using Shared.Models;

public class UserDetails
{
	public required string Name { get; set; }
	public required string Email { get; set; }
	public List<Place> VisitedPlaces { get; set; } = new();
	public List<UserActivity> Activities { get; set; } = new();
}