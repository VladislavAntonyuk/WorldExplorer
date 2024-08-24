namespace WorldExplorer.Modules.Users.IntegrationEvents;

using Common.Application.EventBus;

public sealed class UserDeletedIntegrationEvent(Guid id, DateTime occurredOnUtc, Guid userId)
	: IntegrationEvent(id, occurredOnUtc)
{
	public Guid UserId { get; init; } = userId;
}