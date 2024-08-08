namespace WorldExplorer.Modules.Users.Infrastructure.Outbox;

internal sealed class OutboxOptions
{
    public int IntervalInSeconds { get; init; }

    public int BatchSize { get; init; }
}
