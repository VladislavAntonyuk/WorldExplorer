namespace WorldExplorer.Modules.Users.IntegrationEvents;

using Common.Application.EventBus;

public sealed class UserDeletedIntegrationEvent : IntegrationEvent
{
	public UserDeletedIntegrationEvent(
		Guid id,
		DateTime occurredOnUtc,
		Guid userId)
		: base(id, occurredOnUtc)
	{
		UserId = userId;
	}

	public Guid UserId { get; init; }
}