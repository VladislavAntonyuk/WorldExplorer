namespace WorldExplorer.Modules.Travellers.Infrastructure.Inbox;

using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Serialization;
using Database;
using MassTransit;
using Newtonsoft.Json;

internal sealed class IntegrationEventConsumer<TIntegrationEvent>(TravellersDbContext dbConnectionFactory)
	: IConsumer<TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
{
	public async Task Consume(ConsumeContext<TIntegrationEvent> context)
	{
		var integrationEvent = context.Message;

		var inboxMessage = new InboxMessage
		{
			Id = integrationEvent.Id,
			Type = integrationEvent.GetType().Name,
			Content = JsonConvert.SerializeObject(integrationEvent, SerializerSettings.JsonSerializerSettingsInstance),
			OccurredOnUtc = integrationEvent.OccurredOnUtc
		};

		dbConnectionFactory.InboxMessages.Add(inboxMessage);
		await dbConnectionFactory.SaveChangesAsync();
	}
}