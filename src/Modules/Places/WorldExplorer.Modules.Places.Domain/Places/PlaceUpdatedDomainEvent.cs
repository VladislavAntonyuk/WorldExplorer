namespace WorldExplorer.Modules.Places.Domain.Places;

using Common.Domain;
using NetTopologySuite.Geometries;

public class PlaceUpdatedDomainEvent(Guid id, string name, Point location, string? description) : DomainEvent
{
	public Guid PlaceId { get; init; } = id;
	public string Name { get; init; } = name;
	public Point Location { get; init; } = location;
	public string? Description { get; init; } = description;
}