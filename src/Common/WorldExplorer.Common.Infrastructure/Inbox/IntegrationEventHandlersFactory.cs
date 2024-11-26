namespace WorldExplorer.Common.Infrastructure.Inbox;

using System.Collections.Concurrent;
using System.Reflection;
using Application.EventBus;
using Microsoft.Extensions.DependencyInjection;

public static class IntegrationEventHandlersFactory
{
	private static readonly ConcurrentDictionary<string, Type[]> HandlersDictionary = new();

	public static IEnumerable<IIntegrationEventHandler> GetHandlers(Type type,
		IServiceProvider serviceProvider,
		Assembly assembly)
	{
		var integrationEventHandlerTypes = HandlersDictionary.GetOrAdd($"{assembly.GetName().Name}-{type.Name}", _ =>
		{
			var integrationEventHandlers = assembly.GetTypes()
			                                       .Where(t => t.IsAssignableTo(
				                                              typeof(IIntegrationEventHandler<>).MakeGenericType(type)))
			                                       .ToArray();

			return integrationEventHandlers;
		});

		List<IIntegrationEventHandler> handlers = [];
		foreach (var integrationEventHandlerType in integrationEventHandlerTypes)
		{
			var integrationEventHandler = serviceProvider.GetRequiredService(integrationEventHandlerType);

			handlers.Add((integrationEventHandler as IIntegrationEventHandler)!);
		}

		return handlers;
	}
}