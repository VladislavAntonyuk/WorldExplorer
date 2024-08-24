namespace WorldExplorer.Modules.Travellers;

using Abstractions.Data;
using Application.Travellers.GetById;
using Application.Travellers.GetTravellers;
using Common.Infrastructure;
using Infrastructure.Database;
using Infrastructure.Inbox;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Users.IntegrationEvents;

public static class TravellersModule
{
	public static IHostApplicationBuilder AddTravellersModule(this IHostApplicationBuilder builder)
	{
		//builder.Services.AddDomainEventHandlers();

		// builder.Services.AddIntegrationEventHandlers();

		builder.AddInfrastructure();

		return builder;
	}

	public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
	{
		registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserRegisteredIntegrationEvent>>();
	}

	private static void AddInfrastructure(this IHostApplicationBuilder builder)
	{
		builder.AddDatabase<TravellersDbContext>(Schemas.Travellers);

		builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TravellersDbContext>());

		builder.Services.AddScoped<GetTravellersHandler>();
		builder.Services.AddScoped<GetTravellerByIdHandler>();

		builder.Services.AddGraphQLServer()
		       .RegisterDbContextFactory<TravellersDbContext>()
		       .AddQueryType(d => d.Name("Travellers"))
		       .AddType<GetTravellersHandler>()
		       .AddType<GetTravellerByIdHandler>()
		       .AddQueryConventions()
		       .AddFiltering()
		       .AddSorting();
	}
}