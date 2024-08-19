namespace WorldExplorer.Modules.Places.Domain.Places;

using NetTopologySuite.Geometries;
using WorldExplorer.Common.Domain;

public class PlaceUpdatedDomainEvent(Guid id, string name, Point location, string? description) : DomainEvent
{
	public Guid Id { get; init; } = id;
	public string Name { get; init; } = name;
	public Point Location { get; init; } = location;
	public string? Description { get; init; } = description;
}