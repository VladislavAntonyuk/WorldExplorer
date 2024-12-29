namespace WorldExplorer.Modules.Travellers.Infrastructure.Inbox;

using System.Text.Json;
using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Serialization;
using Database;
using MassTransit;

internal sealed class IntegrationEventConsumer<TIntegrationEvent>(TravellersDbContext dbContext)
	: IConsumer<TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
{
	public async Task Consume(ConsumeContext<TIntegrationEvent> context)
	{
		var integrationEvent = context.Message;

		var inboxMessage = new InboxMessage
		{
			Id = integrationEvent.Id,
			Content = JsonSerializer.Serialize<IIntegrationEvent>(integrationEvent, SerializerSettings.Instance),
			OccurredOnUtc = integrationEvent.OccurredOnUtc
		};

		dbContext.InboxMessages.Add(inboxMessage);
		await dbContext.SaveChangesAsync();
	}
}