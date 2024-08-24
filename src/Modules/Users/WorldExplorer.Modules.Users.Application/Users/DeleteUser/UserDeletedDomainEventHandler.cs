namespace WorldExplorer.Modules.Users.Application.Users.DeleteUser;

using WorldExplorer.Common.Application.EventBus;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Modules.Users.Domain.Users;
using WorldExplorer.Modules.Users.IntegrationEvents;

internal sealed class UserDeletedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<UserDeletedDomainEvent>
{
    public override async Task Handle(
        UserDeletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new UserDeletedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                domainEvent.UserId),
            cancellationToken);
    }
}
