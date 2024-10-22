namespace WorldExplorer.Modules.Users.Infrastructure.Outbox;

using Common.Application.Messaging;
using Common.Domain;
using Common.Infrastructure.Outbox;
using Database;
using Microsoft.EntityFrameworkCore;

internal sealed class IdempotentDomainEventHandler<TDomainEvent>(
	IDomainEventHandler<TDomainEvent> decorated,
	UsersDbContext dbConnectionFactory) : DomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
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

	private async Task<bool> OutboxConsumerExistsAsync(OutboxMessageConsumer outboxMessageConsumer)
	{
		return await dbConnectionFactory.OutboxMessagesConsumers
		                                .AnyAsync(x=>x.Name == outboxMessageConsumer.Name && x.OutboxMessageId == outboxMessageConsumer.OutboxMessageId);
	}

	private async Task InsertOutboxConsumerAsync(OutboxMessageConsumer outboxMessageConsumer)
	{
		dbConnectionFactory.OutboxMessagesConsumers.Add(outboxMessageConsumer);
		await dbConnectionFactory.SaveChangesAsync();
	}
}