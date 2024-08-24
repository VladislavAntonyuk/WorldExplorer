namespace WorldExplorer.Modules.Users.Infrastructure.Outbox;

using System.Text.Json;
using Application;
using Common.Application.Messaging;
using Common.Domain;
using Common.Infrastructure.Outbox;
using Common.Infrastructure.Serialization;
using Database;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

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

		IReadOnlyList<OutboxMessageResponse> outboxMessages = await GetOutboxMessagesAsync();
		await using var transaction = await dbConnectionFactory.Database.BeginTransactionAsync();


		foreach (OutboxMessageResponse outboxMessage in outboxMessages)
		{
			Exception? exception = null;

			try
			{
				var t = typeof(UserProfileUpdatedDomainEvent).Assembly.GetType(outboxMessage.Type);
				IDomainEvent domainEvent = (IDomainEvent)JsonSerializer.Deserialize(
					outboxMessage.Content,
					t,
					SerializerSettings.Instance)!;

				using IServiceScope scope = serviceScopeFactory.CreateScope();

				IEnumerable<IDomainEventHandler> handlers = DomainEventHandlersFactory.GetHandlers(
					domainEvent.GetType(),
					scope.ServiceProvider,
					AssemblyReference.Assembly);

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

			await UpdateOutboxMessageAsync(outboxMessage, exception);
		}

		await transaction.CommitAsync();

		logger.LogInformation("{Module} - Completed processing outbox messages", ModuleName);
	}

	private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync()
	{
		//string sql =
		//    $"""
		//     SELECT TOP {outboxOptions.Value.BatchSize}
		//        id AS {nameof(OutboxMessageResponse.Id)},
		//        content AS {nameof(OutboxMessageResponse.Content)}
		//     FROM users.outbox_messages WITH (UPDLOCK, ROWLOCK)
		//     WHERE ProcessedOnUtc IS NULL
		//     ORDER BY OccurredOnUtc;

		//     """;

		string sql =
			$"""
             SELECT TOP {outboxOptions.Value.BatchSize}
                id AS {nameof(OutboxMessageResponse.Id)},
                type AS {nameof(OutboxMessageResponse.Type)},
                content AS {nameof(OutboxMessageResponse.Content)}
             FROM users.outbox_messages
             WHERE ProcessedOnUtc IS NULL
             ORDER BY OccurredOnUtc
             """;

		IEnumerable<OutboxMessageResponse> outboxMessages = await dbConnectionFactory.Database.SqlQueryRaw<OutboxMessageResponse>(
			sql).ToListAsync();

		return outboxMessages.ToList();
	}

	private async Task UpdateOutboxMessageAsync(
		OutboxMessageResponse outboxMessage,
		Exception? exception)
	{
		await dbConnectionFactory.Database.ExecuteSqlAsync(
			$"""
			 UPDATE users.outbox_messages
			 SET ProcessedOnUtc = "{dateTimeProvider.GetUtcNow()}",
			     Error = "{exception?.ToString()}"
			 WHERE Id = "{outboxMessage.Id}"
			 """);
	}

	internal sealed record OutboxMessageResponse(Guid Id, string Content, string Type);
}
