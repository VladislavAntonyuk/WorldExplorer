namespace WorldExplorer.Modules.Users.Infrastructure.Outbox;

using Common.Application.Messaging;
using Common.Domain;
using Common.Infrastructure.Outbox;
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
        var outboxMessageConsumer = new OutboxMessageConsumer(domainEvent.Id, decorated.GetType().Name);

        if (await OutboxConsumerExistsAsync(outboxMessageConsumer))
        {
            return;
        }

        await decorated.Handle(domainEvent, cancellationToken);

        await InsertOutboxConsumerAsync(outboxMessageConsumer);
    }

    private async Task<bool> OutboxConsumerExistsAsync(
        OutboxMessageConsumer outboxMessageConsumer)
    {
        return await dbConnectionFactory.Database.SqlQuery<int>($"""
                                                                     SELECT 1
                                                                     FROM users.outbox_message_consumers
                                                                     WHERE OutboxMessageId = "{outboxMessageConsumer.OutboxMessageId}" AND
                                                                           Name = "{outboxMessageConsumer.Name}"
                                                                 """).AnyAsync();
    }

    private async Task InsertOutboxConsumerAsync(OutboxMessageConsumer outboxMessageConsumer)
    {
        await dbConnectionFactory.Database.ExecuteSqlAsync($"""
                                                              INSERT INTO users.outbox_message_consumers(OutboxMessageId, Name)
                                                              VALUES ({outboxMessageConsumer.OutboxMessageId}, {outboxMessageConsumer.Name})
                                                              """);
    }
}
