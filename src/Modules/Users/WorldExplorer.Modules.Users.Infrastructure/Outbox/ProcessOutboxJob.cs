using System.Data;
using System.Data.Common;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Common.Infrastructure.Outbox;
using WorldExplorer.Common.Infrastructure.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace WorldExplorer.Modules.Users.Infrastructure.Outbox;

using System.Text.Json;
using Database;
using Microsoft.EntityFrameworkCore;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxJob(
    UsersDbContext dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    TimeProvider dateTimeProvider,
    IOptions<OutboxOptions> outboxOptions,
    ILogger<ProcessOutboxJob> logger) : IJob
{
    private const string ModuleName = "Users";

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Module} - Beginning to process outbox messages", ModuleName);

        await using DbConnection connection = dbConnectionFactory.Database.GetDbConnection();
        await using DbTransaction transaction = await connection.BeginTransactionAsync();

        IReadOnlyList<OutboxMessageResponse> outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        foreach (OutboxMessageResponse outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                IDomainEvent domainEvent = JsonSerializer.Deserialize<IDomainEvent>(
                    outboxMessage.Content,
                    SerializerSettings.Instance)!;

                using IServiceScope scope = serviceScopeFactory.CreateScope();

                IEnumerable<IDomainEventHandler> handlers = DomainEventHandlersFactory.GetHandlers(
                    domainEvent.GetType(),
                    scope.ServiceProvider,
                    Application.AssemblyReference.Assembly);

                foreach (IDomainEventHandler domainEventHandler in handlers)
                {
                    await domainEventHandler.Handle(domainEvent, context.CancellationToken);
                }
            }
            catch (Exception caughtException)
            {
                logger.LogError(
                    caughtException,
                    "{Module} - Exception while processing outbox message {MessageId}",
                    ModuleName,
                    outboxMessage.Id);

                exception = caughtException;
            }

            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }

        await transaction.CommitAsync();

        logger.LogInformation("{Module} - Completed processing outbox messages", ModuleName);
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        string sql =
            $"""
             SELECT
                id AS {nameof(OutboxMessageResponse.Id)},
                content AS {nameof(OutboxMessageResponse.Content)}
             FROM users.outbox_messages
             WHERE processed_on_utc IS NULL
             ORDER BY occurred_on_utc
             LIMIT {outboxOptions.Value.BatchSize}
             FOR UPDATE
             """;

        IEnumerable<OutboxMessageResponse> outboxMessages = await dbConnectionFactory.Database.SqlQueryRaw<OutboxMessageResponse>(
	        sql).ToListAsync();

        return outboxMessages.ToList();
    }

    private async Task UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
	    string sql =
		    $"""
		     UPDATE users.outbox_messages
		     SET processed_on_utc = {dateTimeProvider.GetUtcNow()},
		         error = {exception}
		     WHERE id = {outboxMessage.Id}
		     """;

	    await dbConnectionFactory.Database.ExecuteSqlRawAsync(
		    sql);
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
