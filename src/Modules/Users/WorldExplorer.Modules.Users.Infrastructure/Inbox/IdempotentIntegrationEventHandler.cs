namespace WorldExplorer.Modules.Users.Infrastructure.Inbox;

using System.Data.Common;
using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Database;
using Microsoft.EntityFrameworkCore;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
	IIntegrationEventHandler<TIntegrationEvent> decorated,
	UsersDbContext dbConnectionFactory)
	: IntegrationEventHandler<TIntegrationEvent> where TIntegrationEvent : IIntegrationEvent
{
	public override async Task Handle(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
	{
		await using var connection = dbConnectionFactory.Database.GetDbConnection();

		var inboxMessageConsumer = new InboxMessageConsumer(integrationEvent.Id, decorated.GetType().Name);

		if (await InboxConsumerExistsAsync(connection, inboxMessageConsumer))
		{
			return;
		}

		await decorated.Handle(integrationEvent, cancellationToken);

		await InsertInboxConsumerAsync(connection, inboxMessageConsumer);
	}

	private async Task<bool> InboxConsumerExistsAsync(DbConnection dbConnection,
		InboxMessageConsumer inboxMessageConsumer)
	{
		var sql = $"""
		               SELECT 1
		               FROM users.inbox_message_consumers
		               WHERE inbox_message_id = {inboxMessageConsumer.InboxMessageId} AND
		                     name = {inboxMessageConsumer.Name}
		           """;

		return await dbConnectionFactory.Database.SqlQueryRaw<int>(sql).AnyAsync();
	}

	private async Task InsertInboxConsumerAsync(DbConnection dbConnection, InboxMessageConsumer inboxMessageConsumer)
	{
		var sql = $"""
		           INSERT INTO users.inbox_message_consumers(inbox_message_id, name)
		           VALUES ({inboxMessageConsumer.InboxMessageId}, {inboxMessageConsumer.Name})
		           """;

		await dbConnectionFactory.Database.ExecuteSqlRawAsync(sql);
	}
}