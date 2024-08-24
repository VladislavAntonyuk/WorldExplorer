namespace WorldExplorer.Common.Infrastructure.Outbox;

using System.Collections.Concurrent;
using System.Reflection;
using Application.Messaging;
using Microsoft.Extensions.DependencyInjection;

public static class DomainEventHandlersFactory
{
	private static readonly ConcurrentDictionary<string, Type[]> HandlersDictionary = new();

	public static IEnumerable<IDomainEventHandler> GetHandlers(Type type,
		IServiceProvider serviceProvider,
		Assembly assembly)
	{
		var domainEventHandlerTypes = HandlersDictionary.GetOrAdd($"{assembly.GetName().Name}{type.Name}", _ =>
		{
			var domainEventHandlerTypes = assembly.GetTypes()
			                                      .Where(t => t.IsAssignableTo(
				                                             typeof(IDomainEventHandler<>).MakeGenericType(type)))
			                                      .ToArray();

			return domainEventHandlerTypes;
		});

		List<IDomainEventHandler> handlers = [];
		foreach (var domainEventHandlerType in domainEventHandlerTypes)
		{
			var domainEventHandler = serviceProvider.GetRequiredService(domainEventHandlerType);

			handlers.Add((domainEventHandler as IDomainEventHandler)!);
		}

		return handlers;
	}
}