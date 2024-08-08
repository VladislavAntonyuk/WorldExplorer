using System.Data.Common;
using WorldExplorer.Common.Application.EventBus;
using WorldExplorer.Common.Infrastructure.Inbox;
using WorldExplorer.Common.Infrastructure.Serialization;
using MassTransit;

namespace WorldExplorer.Modules.Users.Infrastructure.Inbox;

using System.Text.Json;
using Database;
using Microsoft.EntityFrameworkCore;

internal sealed class IntegrationEventConsumer<TIntegrationEvent>(UsersDbContext dbConnectionFactory)
    : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        await using DbConnection connection = dbConnectionFactory.Database.GetDbConnection();

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
            INSERT INTO users.inbox_messages(id, type, content, occurred_on_utc)
            VALUES (@Id, @Type, @Content::json, @OccurredOnUtc)
            """;

        await dbConnectionFactory.Database.ExecuteSqlRawAsync(sql, inboxMessage);
    }
}
