namespace WorldExplorer.Modules.Users.Application.Users.UpdateUser;

using Common.Application.EventBus;
using Common.Application.Messaging;
using Domain.Users;
using IntegrationEvents;

internal sealed class UserProfileUpdatedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<UserProfileUpdatedDomainEvent>
{
    public override async Task Handle(
        UserProfileUpdatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new UserProfileUpdatedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                domainEvent.UserId,
                domainEvent.Settings.TrackUserLocation),
            cancellationToken);
    }
}
