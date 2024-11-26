namespace WorldExplorer.Modules.Users.Domain.Users;

using Common.Domain;

public sealed class UserRegisteredDomainEvent(Guid userId) : DomainEvent
{
	public Guid UserId { get; init; } = userId;
}