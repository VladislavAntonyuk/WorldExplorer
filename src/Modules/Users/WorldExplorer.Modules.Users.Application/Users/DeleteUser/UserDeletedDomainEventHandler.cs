namespace WorldExplorer.Modules.Users.Application.Users.DeleteUser;

using Common.Application.EventBus;
using Common.Application.Messaging;
using Domain.Users;
using IntegrationEvents;

internal sealed class UserDeletedDomainEventHandler(IEventBus eventBus) : DomainEventHandler<UserDeletedDomainEvent>
{
	public override async Task Handle(UserDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
	{
		await eventBus.PublishAsync(
			new UserDeletedIntegrationEvent(domainEvent.Id, domainEvent.OccurredOnUtc, domainEvent.UserId),
			cancellationToken);
	}
}