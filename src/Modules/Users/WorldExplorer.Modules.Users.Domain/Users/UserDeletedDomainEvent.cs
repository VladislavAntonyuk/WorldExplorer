namespace WorldExplorer.Modules.Users.Domain.Users;

using Common.Domain;

public sealed class UserDeletedDomainEvent(Guid userId) : DomainEvent
{
	public Guid UserId { get; init; } = userId;
}