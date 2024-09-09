namespace WorldExplorer.Common.Infrastructure.Outbox;

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serialization;

public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = default)
	{
		if (eventData.Context is not null)
		{
			InsertOutboxMessages(eventData.Context);
		}

		return base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private static void InsertOutboxMessages(DbContext context)
	{
		var outboxMessages = context.ChangeTracker.Entries<Entity>()
		                            .Select(entry => entry.Entity)
		                            .SelectMany(entity =>
		                            {
			                            var domainEvents = entity.DomainEvents;

			                            entity.ClearDomainEvents();

			                            return domainEvents;
		                            })
		                            .Select(domainEvent => new OutboxMessage
		                            {
			                            Id = domainEvent.Id,
			                            Type = domainEvent.GetType().FullName,
			                            Content = JsonSerializer.Serialize(domainEvent,
				                                                               domainEvent.GetType(),
				                                                               SerializerSettings.Instance),
			                            OccurredOnUtc = domainEvent.OccurredOnUtc
		                            })
		                            .ToList();

		context.Set<OutboxMessage>().AddRange(outboxMessages);
	}
}