namespace WorldExplorer.Modules.Places.Domain.Places;

using WorldExplorer.Common.Domain;

public class PlaceUpdatedDomainEvent(Guid id, string name, Location location, string? description) : DomainEvent
{
	public Guid Id { get; init; } = id;
	public string Name { get; init; } = name;
	public Location Location { get; init; } = location;
	public string? Description { get; init; } = description;
}