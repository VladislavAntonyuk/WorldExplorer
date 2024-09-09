namespace WorldExplorer.Modules.Users.Infrastructure.Inbox;

using System.Text.Json;
using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Serialization;
using Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;

internal sealed class IntegrationEventConsumer<TIntegrationEvent>(UsersDbContext dbConnectionFactory)
	: IConsumer<TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
{
	public async Task Consume(ConsumeContext<TIntegrationEvent> context)
	{
		await using var connection = dbConnectionFactory.Database.GetDbConnection();

		var integrationEvent = context.Message;

		var inboxMessage = new InboxMessage
		{
			Id = integrationEvent.Id,
			Type = integrationEvent.GetType().Name,
			Content = JsonSerializer.Serialize(integrationEvent, SerializerSettings.Instance),
			OccurredOnUtc = integrationEvent.OccurredOnUtc
		};

		dbConnectionFactory.InboxMessages.Add(inboxMessage);
		await dbConnectionFactory.SaveChangesAsync();
	}
}