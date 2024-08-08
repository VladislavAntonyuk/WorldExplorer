namespace WorldExplorer.Modules.Places.Domain.Places;

using WorldExplorer.Common.Domain;

public class PlaceCreatedDomainEvent(Guid placeId) : DomainEvent
{
	public Guid PlaceId { get; init; } = placeId;
}