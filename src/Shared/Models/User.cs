namespace Shared.Models;

public class User
{
	public required string Name { get; set; }
	public required string Email { get; set; }
	public List<Place> VisitedPlaces { get; set; } = new();
}