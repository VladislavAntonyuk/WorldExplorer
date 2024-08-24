namespace WorldExplorer.Modules.Travellers.Infrastructure.Inbox;

using System.Text.Json;
using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Serialization;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

[DisallowConcurrentExecution]
internal sealed class ProcessInboxJob(
	TravellersDbContext dbConnectionFactory,
	IServiceScopeFactory serviceScopeFactory,
	TimeProvider timeProvider,
	IOptions<InboxOptions> inboxOptions,
	ILogger<ProcessInboxJob> logger) : IJob
{
	private const string ModuleName = "Travellers";

	public async Task Execute(IJobExecutionContext context)
	{
		logger.LogInformation("{Module} - Beginning to process inbox messages", ModuleName);

		await using var transaction = await dbConnectionFactory.Database.BeginTransactionAsync();

		var inboxMessages = await GetInboxMessagesAsync();

		foreach (var inboxMessage in inboxMessages)
		{
			Exception? exception = null;

			try
			{
				var integrationEvent = JsonSerializer.Deserialize<IIntegrationEvent>(
					inboxMessage.Content, SerializerSettings.Instance)!;

				using var scope = serviceScopeFactory.CreateScope();

				var handlers = IntegrationEventHandlersFactory.GetHandlers(
					integrationEvent.GetType(), scope.ServiceProvider, AssemblyReference.Assembly);

				foreach (var integrationEventHandler in handlers)
				{
					await integrationEventHandler.Handle(integrationEvent, context.CancellationToken);
				}
			}
			catch (Exception caughtException)
			{
				logger.LogError(caughtException, "{Module} - Exception while processing inbox message {MessageId}",
				                ModuleName, inboxMessage.Id);

				exception = caughtException;
			}

			await UpdateInboxMessageAsync(inboxMessage, exception);
		}

		await transaction.CommitAsync();

		logger.LogInformation("{Module} - Completed processing inbox messages", ModuleName);
	}

	private async Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync()
	{
		var sql = $"""
		           SELECT TOP {inboxOptions.Value.BatchSize}
		              id AS {nameof(InboxMessageResponse.Id)},
		              content AS {nameof(InboxMessageResponse.Content)}
		           FROM travellers.inbox_messages
		           WHERE ProcessedOnUtc IS NULL
		           ORDER BY OccuredOnUtc
		           """;

		IEnumerable<InboxMessageResponse> inboxMessages =
			await dbConnectionFactory.Database.SqlQueryRaw<InboxMessageResponse>(sql).ToListAsync();

		return inboxMessages.ToList();
	}

	private async Task UpdateInboxMessageAsync(InboxMessageResponse inboxMessage, Exception? exception)
	{
		await dbConnectionFactory.Database.ExecuteSqlInterpolatedAsync($"""
		                                                                UPDATE travellers.inbox_messages
		                                                                SET ProcessedOnUtc = {timeProvider.GetUtcNow()},
		                                                                    error = {exception}
		                                                                WHERE id = {inboxMessage.Id}
		                                                                """);
	}

	internal sealed record InboxMessageResponse(Guid Id, string Content);
}