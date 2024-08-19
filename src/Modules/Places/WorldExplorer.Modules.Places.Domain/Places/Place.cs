namespace WorldExplorer.Modules.Places.Domain.Places;

using NetTopologySuite.Geometries;
using WorldExplorer.Common.Domain;

public class Place : Entity
{
	private Place()
	{
		
	}

	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public Point Location { get;private set; }
	public string? Description { get; private set; }

	public ICollection<Image> Images { get; private set; } = new List<Image>();

	public static Place Create(string name, Point location, string? description)
	{
		var place = new Place
		{
			Id = Guid.CreateVersion7(),
			Name = name,
			Location = location,
			Description = description
		};

		place.Raise(new PlaceCreatedDomainEvent(place.Id));

		return place;
	}

	public void Update(string name, Point location, string? description)
	{
		if (Name == name && Location == location && Description == description)
		{
			return;
		}

		Name = name;
		Location = location;
		Description = description;

		Raise(new PlaceUpdatedDomainEvent(Id, Name, Location, Description));
	}
}