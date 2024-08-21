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
    private const string ModuleName = "Users";

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Module} - Beginning to process inbox messages", ModuleName);

        IReadOnlyList<InboxMessageResponse> inboxMessages = await GetInboxMessagesAsync();
        await using var transaction = await dbConnectionFactory.Database.BeginTransactionAsync();


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

            await UpdateInboxMessageAsync(inboxMessage, exception);
        }

        await transaction.CommitAsync();

        logger.LogInformation("{Module} - Completed processing inbox messages", ModuleName);
    }

    private async Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync()
    {
        string sql =
            $"""
             SELECT TOP {inboxOptions.Value.BatchSize}
                id AS {nameof(InboxMessageResponse.Id)},
                content AS {nameof(InboxMessageResponse.Content)}
             FROM users.inbox_messages WITH (UPDLOCK, ROWLOCK)
             WHERE ProcessedOnUtc IS NULL
             ORDER BY OccurredOnUtc;
             
             """;
		
        IEnumerable<InboxMessageResponse> inboxMessages = await dbConnectionFactory.Database.SqlQueryRaw<InboxMessageResponse>(
            sql).ToListAsync();

        return inboxMessages.ToList();
    }

    private async Task UpdateInboxMessageAsync(
        InboxMessageResponse inboxMessage,
        Exception? exception)
    {
        await dbConnectionFactory.Database.ExecuteSqlAsync(
	        $"""
	         UPDATE users.inbox_messages
	         SET ProcessedOnUtc = {timeProvider.GetUtcNow()},
	             error = {exception}
	         WHERE id = {inboxMessage.Id}
	         """);
    }

    internal sealed record InboxMessageResponse(Guid Id, string Content);
}
