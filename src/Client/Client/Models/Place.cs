namespace Shared.Models;

public class Place
{
	public static readonly Place Default = new()
	{
		Name = string.Empty,
		Location = new Location(0, 0)
	};

	public Guid Id { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }

	public ICollection<string> Images { get; set; } = [];

	public required Location Location { get; set; }
	public double Rating => Reviews.Count > 0 ? Reviews.Average(x => x.Rating) : 0;
	public List<Review> Reviews { get; set; } = [];
	public string? MainImage => Images.FirstOrDefault();
}

public sealed record PlaceResponse(
	Guid Id,
	string Name,
	string? Description,
	Location Location,
	ICollection<string> Images)
{
	public string? MainImage => Images.FirstOrDefault();
}

public class Review
{
	public DateTimeOffset ReviewDate { get; init; }
	public string? Comment { get; init; }
	public double Rating { get; init; }
	public TravellerResponse Traveller { get; init; }
}

public record TravellerResponse(Guid Id, string DisplayName);