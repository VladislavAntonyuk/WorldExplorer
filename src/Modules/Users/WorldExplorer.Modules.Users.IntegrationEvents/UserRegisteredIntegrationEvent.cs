using WorldExplorer.Common.Application.EventBus;

namespace WorldExplorer.Modules.Users.IntegrationEvents;

public sealed class UserRegisteredIntegrationEvent : IntegrationEvent
{
    public UserRegisteredIntegrationEvent(
        Guid id,
        DateTime occurredOnUtc,
        Guid userId,
        string email,
        string name,
        string language)
        : base(id, occurredOnUtc)
    {
        UserId = userId;
        Email = email;
        Name = name;
        Language = language;
    }

    public Guid UserId { get; init; }

    public string Email { get; init; }

    public string Name { get; init; }

    public string Language { get; init; }
}