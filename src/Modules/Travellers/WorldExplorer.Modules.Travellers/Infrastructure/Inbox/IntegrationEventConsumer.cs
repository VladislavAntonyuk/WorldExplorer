namespace WorldExplorer.Modules.Travellers.Infrastructure.Inbox;

using System.Text.Json;
using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Serialization;
using Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;

internal sealed class IntegrationEventConsumer<TIntegrationEvent>(TravellersDbContext dbConnectionFactory)
    : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        TIntegrationEvent integrationEvent = context.Message;

        var inboxMessage = new InboxMessage
        {
            Id = integrationEvent.Id,
            Type = integrationEvent.GetType().Name,
            Content = JsonSerializer.Serialize(integrationEvent, SerializerSettings.Instance),
            OccurredOnUtc = integrationEvent.OccurredOnUtc
        };

        const string sql =
            """
            INSERT INTO travellers.inbox_messages(id, type, content, occurred_on_utc)
            VALUES (@Id, @Type, @Content, @OccurredOnUtc)
            """;

        await dbConnectionFactory.Database.ExecuteSqlRawAsync(sql, inboxMessage);
    }
}
