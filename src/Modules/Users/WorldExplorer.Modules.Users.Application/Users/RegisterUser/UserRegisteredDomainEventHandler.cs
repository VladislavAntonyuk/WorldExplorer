namespace WorldExplorer.Modules.Users.Application.Users.RegisterUser;

using Common.Application.EventBus;
using Common.Application.Exceptions;
using Common.Application.Messaging;
using Domain.Users;
using GetUser;
using IntegrationEvents;
using MediatR;

internal sealed class UserRegisteredDomainEventHandler(ISender sender, IEventBus bus)
    : DomainEventHandler<UserRegisteredDomainEvent>
{
    public override async Task Handle(
        UserRegisteredDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetUserQuery(domainEvent.UserId),
            cancellationToken);

        if (result.IsFailure)
        {
            throw new WorldExplorerException(nameof(GetUserQuery), result.Error);
        }

        await bus.PublishAsync(
            new UserRegisteredIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                result.Value.Id,
                result.Value.Email,
                result.Value.Name,
                result.Value.Language.ToString()),
            cancellationToken);
    }
}
