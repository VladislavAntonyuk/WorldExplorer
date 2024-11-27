namespace WorldExplorer.Modules.Users.Infrastructure.Outbox;

using Application;
using Common.Domain;
using Common.Infrastructure.Outbox;
using Common.Infrastructure.Serialization;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

		var outboxMessages = await GetOutboxMessagesAsync();
		//		await using var transaction = await dbConnectionFactory.Database.BeginTransactionAsync();


		foreach (var outboxMessage in outboxMessages)
		{
			Exception? exception = null;

			try
			{
				var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
					outboxMessage.Content, SerializerSettings.JsonSerializerSettingsInstance)!;

				using var scope = serviceScopeFactory.CreateScope();

				var handlers = DomainEventHandlersFactory.GetHandlers(
					domainEvent.GetType(), scope.ServiceProvider, AssemblyReference.Assembly);

				foreach (var domainEventHandler in handlers)
				{
					await domainEventHandler.Handle(domainEvent, context.CancellationToken);
				}
			}
			catch (Exception caughtException)
			{
				logger.LogError(caughtException, "{Module} - Exception while processing outbox message {MessageId}",
				                ModuleName, outboxMessage.Id);

				exception = caughtException;
			}

			await UpdateOutboxMessageAsync(outboxMessage, exception);
		}

		//await transaction.CommitAsync();

		logger.LogInformation("{Module} - Completed processing outbox messages", ModuleName);
	}

	private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync()
	{
		var outboxMessages = await dbConnectionFactory.OutboxMessages.Where(x => x.ProcessedOnUtc == null)
		                                              .Take(outboxOptions.Value.BatchSize)
		                                              .Select(x => new OutboxMessageResponse(x.Id, x.Content, x.Type))
		                                              .ToListAsync();

		return outboxMessages;
	}

	private async Task UpdateOutboxMessageAsync(OutboxMessageResponse outboxMessage, Exception? exception)
	{
		var error = exception?.ToString();
		await dbConnectionFactory.OutboxMessages.Where(x => x.Id == outboxMessage.Id)
		                         .ExecuteUpdateAsync(m => m.SetProperty(p => p.Error, error)
		                                                   .SetProperty(
			                                                   p => p.ProcessedOnUtc,
			                                                   dateTimeProvider.GetUtcNow().UtcDateTime));
	}

	internal sealed record OutboxMessageResponse(Guid Id, string Content, string Type);
}