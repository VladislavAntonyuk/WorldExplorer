namespace WorldExplorer.Modules.Places.Domain.Places;

using Common.Domain;
using NetTopologySuite.Geometries;

public class Place : Entity
{
	public Place(string name, Point location, string? description)
	{
		Id = Guid.CreateVersion7();
		Name = name;
		Location = location;
		Description = description;
		Raise(new PlaceCreatedDomainEvent(Id));
	}

	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public Point Location { get; private set; }
	public string? Description { get; private set; }

	public ICollection<PlaceImage> Images { get; private set; } = new List<PlaceImage>();

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