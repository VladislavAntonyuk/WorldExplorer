using WorldExplorer.Common.Application.EventBus;

namespace WorldExplorer.Modules.Users.IntegrationEvents;

public sealed class UserProfileUpdatedIntegrationEvent : IntegrationEvent
{
	public UserProfileUpdatedIntegrationEvent(
		Guid id,
		DateTime occurredOnUtc,
		Guid userId,
		bool trackUserLocation)
		: base(id, occurredOnUtc)
	{
		UserId = userId;
		TrackUserLocation = trackUserLocation;
	}

	public Guid UserId { get; init; }

	public bool TrackUserLocation { get; init; }
}
