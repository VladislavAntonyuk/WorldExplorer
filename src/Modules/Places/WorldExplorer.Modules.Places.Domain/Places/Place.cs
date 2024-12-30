namespace WorldExplorer.Modules.Places.Domain.Places;

using Common.Domain;
using NetTopologySuite.Geometries;

public class Place : Entity
{
	private Place()
	{
		Name = string.Empty;
		Location = Point.Empty;
	}

	public static Place Create(string name, Point location)
	{
		var place = new Place
		{
			Id = Guid.CreateVersion7(),
			Name = name,
			Location = location,
			CreatedAt = DateTimeOffset.UtcNow
		};

		place.Raise(new PlaceCreatedDomainEvent(place.Id));
		return place;
	}

	public Guid Id { get; private init; }
	public DateTimeOffset CreatedAt { get; private init; }
	public string Name { get; private set; }
	public Point Location { get; private set; }
	public string? Description { get; private set; }

	public ICollection<PlaceImage> Images { get; private set; } = new List<PlaceImage>();

	public void Update(string name, Point location, string? description, ICollection<PlaceImage> images)
	{
		if (Name == name && Location == location && Description == description && Images.Count == images.Count)
		{
			return;
		}

		Name = name;
		Location = location;
		Description = description;
		Images = images;

		Raise(new PlaceUpdatedDomainEvent(Id, Name, Location, Description));
	}

	public void Delete()
	{
		Raise(new PlaceDeletedDomainEvent(Id));
	}
}