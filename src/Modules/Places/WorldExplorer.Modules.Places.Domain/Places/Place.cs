﻿namespace WorldExplorer.Modules.Places.Domain.Places;

using WorldExplorer.Common.Domain;

public class Place : Entity
{
	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public Location Location { get;private set; }
	public string? Description { get; private set; }

	public ICollection<Image> Images { get; private set; } = new List<Image>();

	public static Place Create(string name, Location location, string? description)
	{
		var place = new Place
		{
			Id = Guid.NewGuid(),
			Name = name,
			Location = location,
			Description = description
		};

		place.Raise(new PlaceCreatedDomainEvent(place.Id));

		return place;
	}

	public void Update(string name, Location location, string? description)
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