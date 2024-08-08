using System.Data;
using System.Data.Common;
using WorldExplorer.Common.Application.EventBus;
using WorldExplorer.Common.Infrastructure.Inbox;
using WorldExplorer.Common.Infrastructure.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace WorldExplorer.Modules.Users.Infrastructure.Inbox;

using System.Text.Json;
using Database;
using Microsoft.EntityFrameworkCore;

[DisallowConcurrentExecution]
internal sealed class ProcessInboxJob(
    UsersDbContext dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
	TimeProvider timeProvider,
    IOptions<InboxOptions> inboxOptions,
    ILogger<ProcessInboxJob> logger) : IJob
{
    private const string ModuleName = "Attendance";

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Module} - Beginning to process inbox messages", ModuleName);

        await using DbConnection connection = dbConnectionFactory.Database.GetDbConnection();
        await using DbTransaction transaction = await connection.BeginTransactionAsync();

        IReadOnlyList<InboxMessageResponse> inboxMessages = await GetInboxMessagesAsync(connection, transaction);

        foreach (InboxMessageResponse inboxMessage in inboxMessages)
        {
            Exception? exception = null;

            try
            {
                IIntegrationEvent integrationEvent = JsonSerializer.Deserialize<IIntegrationEvent>(
                    inboxMessage.Content,
                    SerializerSettings.Instance)!;

                using IServiceScope scope = serviceScopeFactory.CreateScope();

                IEnumerable<IIntegrationEventHandler> handlers = IntegrationEventHandlersFactory.GetHandlers(
                    integrationEvent.GetType(),
                    scope.ServiceProvider,
                    Presentation.AssemblyReference.Assembly);

                foreach (IIntegrationEventHandler integrationEventHandler in handlers)
                {
                    await integrationEventHandler.Handle(integrationEvent, context.CancellationToken);
                }
            }
            catch (Exception caughtException)
            {
                logger.LogError(
                    caughtException,
                    "{Module} - Exception while processing inbox message {MessageId}",
                    ModuleName,
                    inboxMessage.Id);

                exception = caughtException;
            }

            await UpdateInboxMessageAsync(connection, transaction, inboxMessage, exception);
        }

        await transaction.CommitAsync();

        logger.LogInformation("{Module} - Completed processing inbox messages", ModuleName);
    }

    private async Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        string sql =
            $"""
             SELECT
                id AS {nameof(InboxMessageResponse.Id)},
                content AS {nameof(InboxMessageResponse.Content)}
             FROM users.inbox_messages
             WHERE processed_on_utc IS NULL
             ORDER BY occurred_on_utc
             LIMIT {inboxOptions.Value.BatchSize}
             FOR UPDATE
             """;
		
        IEnumerable<InboxMessageResponse> inboxMessages = await dbConnectionFactory.Database.SqlQueryRaw<InboxMessageResponse>(
            sql).ToListAsync();

        return inboxMessages.ToList();
    }

    private async Task UpdateInboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        InboxMessageResponse inboxMessage,
        Exception? exception)
    {
        string sql =
            $"""
            UPDATE users.inbox_messages
            SET processed_on_utc = {timeProvider.GetUtcNow()},
                error = {exception}
            WHERE id = {inboxMessage.Id}
            """;

        await dbConnectionFactory.Database.ExecuteSqlRawAsync(
            sql);
    }

    internal sealed record InboxMessageResponse(Guid Id, string Content);
}
