namespace WorldExplorer.Modules.Places.Domain.Places;

using Common.Domain;

public class PlaceDeletedDomainEvent(Guid placeId) : DomainEvent
{
	public Guid PlaceId { get; init; } = placeId;
}