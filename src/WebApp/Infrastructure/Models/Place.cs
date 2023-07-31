namespace WebApp.Infrastructure.Models;

public class Place
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }
	public string? LongDescription { get; set; }
	public List<Image> Images { get; set; } = new();
	public required Location Location { get; set; }
}