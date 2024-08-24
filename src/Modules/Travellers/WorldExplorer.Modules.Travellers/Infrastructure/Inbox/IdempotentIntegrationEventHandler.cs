namespace WorldExplorer.Modules.Travellers.Infrastructure.Inbox;

using System.Data.Common;
using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Database;
using Microsoft.EntityFrameworkCore;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
    IIntegrationEventHandler<TIntegrationEvent> decorated,
    TravellersDbContext dbConnectionFactory)
    : IntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    public override async Task Handle(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = dbConnectionFactory.Database.GetDbConnection();

        var inboxMessageConsumer = new InboxMessageConsumer(integrationEvent.Id, decorated.GetType().Name);

        if (await InboxConsumerExistsAsync(inboxMessageConsumer))
        {
            return;
        }

        await decorated.Handle(integrationEvent, cancellationToken);

        await InsertInboxConsumerAsync(inboxMessageConsumer);
    }

    private async Task<bool> InboxConsumerExistsAsync(InboxMessageConsumer inboxMessageConsumer)
    {
        string sql =
            $"""
                SELECT 1
                FROM travellers.inbox_message_consumers
                WHERE inbox_message_id = {inboxMessageConsumer.InboxMessageId} AND
                      name = {inboxMessageConsumer.Name}
            """;

        return await dbConnectionFactory.Database.SqlQueryRaw<int>(sql).AnyAsync();
    }

    private async Task InsertInboxConsumerAsync(InboxMessageConsumer inboxMessageConsumer)
    {
        string sql =
            $"""
            INSERT INTO travellers.inbox_message_consumers(inbox_message_id, name)
            VALUES ({inboxMessageConsumer.InboxMessageId}, {inboxMessageConsumer.Name})
            """;

        await dbConnectionFactory.Database.ExecuteSqlRawAsync(sql);
    }
}
