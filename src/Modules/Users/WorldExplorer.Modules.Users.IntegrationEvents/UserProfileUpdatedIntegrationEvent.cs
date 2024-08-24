namespace WorldExplorer.Modules.Users.IntegrationEvents;

using Common.Application.EventBus;

public sealed class UserProfileUpdatedIntegrationEvent(
	Guid id,
	DateTime occurredOnUtc,
	Guid userId,
	bool trackUserLocation) : IntegrationEvent(id, occurredOnUtc)
{
	public Guid UserId { get; init; } = userId;

	public bool TrackUserLocation { get; init; } = trackUserLocation;
}