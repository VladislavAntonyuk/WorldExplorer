namespace WorldExplorer.Modules.Users.IntegrationEvents;

using Common.Application.EventBus;

public sealed class UserRegisteredIntegrationEvent(
	Guid id,
	DateTime occurredOnUtc,
	Guid userId,
	string email,
	string name,
	string language) : IntegrationEvent(id, occurredOnUtc)
{
	public Guid UserId { get; init; } = userId;

    public string Email { get; init; } = email;

    public string Name { get; init; } = name;

    public string Language { get; init; } = language;
}