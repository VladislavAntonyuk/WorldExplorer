namespace WebApp.Infrastructure.Models;

using NetTopologySuite.Geometries;

public class Place
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public required Point Location { get; set; }
	public string? Description { get; set; }
	public ICollection<Image> Images { get; set; } = new List<Image>();
	public ICollection<Review> Reviews { get; set; } = new List<Review>();
}