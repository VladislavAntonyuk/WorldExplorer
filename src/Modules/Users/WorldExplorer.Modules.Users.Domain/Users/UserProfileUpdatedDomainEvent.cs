namespace WorldExplorer.Modules.Users.Domain.Users;

using Common.Domain;

public sealed class UserProfileUpdatedDomainEvent(Guid userId, UserSettings settings) : DomainEvent
{
    public Guid UserId { get; init; } = userId;

    public UserSettings Settings { get; init; } = settings;
}
