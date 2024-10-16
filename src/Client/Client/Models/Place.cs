namespace Shared.Models;

public class Place
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }

	public List<string> Images { get; set; } = [];

	public required Location Location { get; set; }
	public double Rating => Reviews.Count > 0 ? Reviews.Average(x => x.Rating) : 0;
	public ICollection<Review> Reviews { get; set; } = new List<Review>();
	public string? MainImage => Images.FirstOrDefault();
	public static readonly Place Default = new()
	{
		Name = string.Empty,
		Location = new Location(0, 0)
	};
}

public sealed record PlaceResponse(
	Guid Id,
	string Name,
	string? Description,
	Location Location,
	double Rating,
	ICollection<string> Images)
{
	public string? MainImage => Images.FirstOrDefault();
}

public class Review
{
	public double Rating { get; set; }
}