namespace WorldExplorer.Modules.Users.Infrastructure.Inbox;

internal sealed class InboxOptions
{
	public int IntervalInSeconds { get; init; }

	public int BatchSize { get; init; }
}