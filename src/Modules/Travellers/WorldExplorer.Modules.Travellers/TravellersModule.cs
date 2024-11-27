namespace WorldExplorer.Modules.Travellers;

using Abstractions.Data;
using Application.Travellers;
using Application.Travellers.GetById;
using Application.Travellers.GetTravellers;
using Application.Visits;
using Application.Visits.GetVisits;
using Common.Application.EventBus;
using Common.Infrastructure;
using Infrastructure.Database;
using Infrastructure.Inbox;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Places.IntegrationEvents;
using Users.IntegrationEvents;

public static class TravellersModule
{
	public static IHostApplicationBuilder AddTravellersModule(this IHostApplicationBuilder builder)
	{
		builder.Services.AddIntegrationEventHandlers();

		builder.AddInfrastructure();


		builder.Services.Configure<InboxOptions>(builder.Configuration.GetSection("Travellers:Inbox"));

		builder.Services.ConfigureOptions<ConfigureProcessInboxJob>();
		return builder;
	}

	private static void AddIntegrationEventHandlers(this IServiceCollection services)
	{
		var integrationEventHandlers = AssemblyReference.Assembly.GetTypes()
		                                                .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
		                                                .ToArray();

		foreach (var integrationEventHandler in integrationEventHandlers)
		{
			services.TryAddScoped(integrationEventHandler);

			var integrationEvent = integrationEventHandler.GetInterfaces()
			                                              .Single(i => i.IsGenericType)
			                                              .GetGenericArguments()
			                                              .Single();

			var closedIdempotentHandler = typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

			try
			{
				services.Decorate(integrationEventHandler, closedIdempotentHandler);
			}
			catch (Exception e)
			{
				// todo fix
				Console.WriteLine(e);
			}
		}
	}

	public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
	{
		registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserRegisteredIntegrationEvent>>();
		registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserDeletedIntegrationEvent>>();
		registrationConfigurator.AddConsumer<IntegrationEventConsumer<PlaceCreatedIntegrationEvent>>();
		registrationConfigurator.AddConsumer<IntegrationEventConsumer<PlaceDeletedIntegrationEvent>>();
	}

	private static void AddInfrastructure(this IHostApplicationBuilder builder)
	{
		builder.AddDatabase<TravellersDbContext>(Schemas.Travellers);

		builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TravellersDbContext>());

		builder.Services.AddScoped<ITravellerRepository, TravellerRepository>();
		builder.Services.AddScoped<IPlaceRepository, PlaceRepository>();
		builder.Services.AddScoped<GetTravellersHandler>();
		builder.Services.AddScoped<GetTravellerByIdHandler>();
		builder.Services.AddScoped<GetVisitsHandler>();

		builder.Services.AddGraphQLServer()
		       .RegisterDbContextFactory<TravellersDbContext>()
		       .AddQueryType(d => d.Name("Travellers"))
		       .AddType<GetTravellersHandler>()
		       .AddType<GetTravellerByIdHandler>()
		       .AddType<GetVisitsHandler>()
		       .AddQueryConventions()
		       .AddFiltering()
		       .AddSorting();
	}
}