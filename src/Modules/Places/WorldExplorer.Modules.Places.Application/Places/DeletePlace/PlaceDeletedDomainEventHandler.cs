namespace WorldExplorer.Modules.Places.Application.Places.DeletePlace;

using Common.Application.EventBus;
using Common.Application.Messaging;
using Domain.Places;
using IntegrationEvents;

internal sealed class PlaceDeletedDomainEventHandler(IEventBus eventBus) : DomainEventHandler<PlaceDeletedDomainEvent>
{
	public override async Task Handle(PlaceDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
	{
		await eventBus.PublishAsync(
			new PlaceDeletedIntegrationEvent(domainEvent.Id, domainEvent.OccurredOnUtc, domainEvent.PlaceId),
			cancellationToken);
	}
}