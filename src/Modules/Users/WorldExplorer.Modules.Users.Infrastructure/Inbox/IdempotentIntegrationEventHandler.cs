namespace WorldExplorer.Modules.Users.Infrastructure.Inbox;

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
		return await dbConnectionFactory.InboxMessagesConsumers.AnyAsync(
			x => x.Name == inboxMessageConsumer.Name && x.InboxMessageId == inboxMessageConsumer.InboxMessageId);
	}

	private async Task InsertInboxConsumerAsync(InboxMessageConsumer inboxMessageConsumer)
	{
		dbConnectionFactory.InboxMessagesConsumers.Add(inboxMessageConsumer);
		await dbConnectionFactory.SaveChangesAsync();
	}
}