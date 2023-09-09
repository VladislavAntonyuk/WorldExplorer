namespace Shared.Models;

public class Place
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }
	public List<string> Images { get; set; } = new();
	public required Location Location { get; set; }
	public double Rating => Reviews.Count > 0 ? Reviews.Average(x => x.Rating) : 0;
	public ICollection<Review> Reviews { get; set; } = new List<Review>();
	public string? MainImage => Images.FirstOrDefault();
	public static readonly Place Default = new()
	{
		Name = string.Empty,
		Location = Location.Default
	};
}