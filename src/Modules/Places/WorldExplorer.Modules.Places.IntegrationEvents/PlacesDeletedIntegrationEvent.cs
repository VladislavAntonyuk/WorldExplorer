namespace WorldExplorer.Modules.Places.IntegrationEvents;

using Common.Application.EventBus;

public sealed class PlacesDeletedIntegrationEvent(Guid id, DateTime occurredOnUtc)
	: IntegrationEvent(id, occurredOnUtc);