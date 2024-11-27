namespace WorldExplorer.Modules.Places.Application.Places.DeletePlace;

using Common.Application.EventBus;
using Common.Application.Messaging;
using Domain.Places;
using IntegrationEvents;

internal sealed class PlaceCreatedDomainEventHandler(IEventBus eventBus) : DomainEventHandler<PlaceCreatedDomainEvent>
{
	public override async Task Handle(PlaceCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
	{
		await eventBus.PublishAsync(
			new PlaceCreatedIntegrationEvent(domainEvent.Id, domainEvent.OccurredOnUtc, domainEvent.PlaceId),
			cancellationToken);
	}
}