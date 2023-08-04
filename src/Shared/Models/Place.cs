namespace Shared.Models;

public class Place
{
	public required string Name { get; set; }
	public string? Description { get; set; }
	public required Location Location { get; set; }
	public string? MainImage => Images.FirstOrDefault();
	public List<string> Images { get; set; } = new();
	public double Rating { get; set; }
	public static readonly Place Default = new (){ Name = string.Empty, Location = Location.Default };
}