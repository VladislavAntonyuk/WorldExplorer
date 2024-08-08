using System.Data.Common;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Common.Infrastructure.Outbox;

namespace WorldExplorer.Modules.Users.Infrastructure.Outbox;

using Database;
using Microsoft.EntityFrameworkCore;

internal sealed class IdempotentDomainEventHandler<TDomainEvent>(
    IDomainEventHandler<TDomainEvent> decorated,
    UsersDbContext dbConnectionFactory)
    : DomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    public override async Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = dbConnectionFactory.Database.GetDbConnection();

        var outboxMessageConsumer = new OutboxMessageConsumer(domainEvent.Id, decorated.GetType().Name);

        if (await OutboxConsumerExistsAsync(connection, outboxMessageConsumer))
        {
            return;
        }

        await decorated.Handle(domainEvent, cancellationToken);

        await InsertOutboxConsumerAsync(connection, outboxMessageConsumer);
    }

    private async Task<bool> OutboxConsumerExistsAsync(
        DbConnection dbConnection,
        OutboxMessageConsumer outboxMessageConsumer)
    {
        string sql = 
            $"""
            SELECT EXISTS(
                SELECT 1
                FROM users.outbox_message_consumers
                WHERE outbox_message_id = {outboxMessageConsumer.OutboxMessageId} AND
                      name = {outboxMessageConsumer.Name}
            )
            """;

        return await dbConnectionFactory.Database.SqlQueryRaw<int>(sql, outboxMessageConsumer).AnyAsync();
    }

    private async Task InsertOutboxConsumerAsync(
        DbConnection dbConnection,
        OutboxMessageConsumer outboxMessageConsumer)
    {
        const string sql =
            """
            INSERT INTO users.outbox_message_consumers(outbox_message_id, name)
            VALUES (@OutboxMessageId, @Name)
            """;

        await dbConnectionFactory.Database.ExecuteSqlRawAsync(sql, outboxMessageConsumer);
    }
}
