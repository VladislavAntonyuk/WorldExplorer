namespace WorldExplorer.Modules.Places.IntegrationEvents;

using Common.Application.EventBus;

public sealed class PlaceCreatedIntegrationEvent(Guid id, DateTime occurredOnUtc, Guid placeId)
	: IntegrationEvent(id, occurredOnUtc)
{
	public Guid PlaceId { get; init; } = placeId;
}