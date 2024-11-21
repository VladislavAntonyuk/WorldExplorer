namespace WorldExplorer.Modules.Places.Infrastructure;

using AI;
using Application.Abstractions;
using Application.Abstractions.Data;
using Common.Infrastructure;
using Common.Presentation.Endpoints;
using Database;
using Domain.LocationInfo;
using Domain.Places;
using Image;
using LocationInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using OpenAI;
using Places;
using Presentation;
using WorldExplorer.Common.Application.EventBus;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Modules.Users.Infrastructure.Inbox;

public static class PlacesModule
{
	public static IHostApplicationBuilder AddPlacesModule(this IHostApplicationBuilder builder)
	{
		builder.Services.AddDomainEventHandlers();

		builder.Services.AddIntegrationEventHandlers();

		builder.AddInfrastructure();

		builder.Services.AddEndpoints(AssemblyReference.Assembly);

		return builder;
	}

	private static void AddInfrastructure(this IHostApplicationBuilder builder)
	{
		builder.AddDatabase<PlacesDbContext>(Schemas.Places, null, options => options.UseNetTopologySuite());

		builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<PlacesDbContext>());

		builder.Services.AddScoped<IPlaceRepository, PlaceRepository>();
		builder.Services.AddScoped<ILocationInfoRepository, LocationInfoRepository>();

		if (builder.Environment.IsDevelopment())
		{
			builder.AddOllamaApiClient("ai-llama3-2");
			//builder.AddOllamaApiClient("ai-phi3-5");
		}
		else
		{
			builder.AddOpenAIClient("openai");
		}

		builder.Services.AddScoped<IAiService, AiService>();

		builder.Services.AddHttpClient("GoogleImages", client => client.BaseAddress = new Uri("https://serpapi.com"));
		builder.Services.AddSingleton<IImageSearchService, ImageSearchService>();


		//services.Configure<OutboxOptions>(configuration.GetSection("Users:Outbox"));

		//services.ConfigureOptions<ConfigureProcessOutboxJob>();

		//builder.Services.Configure<InboxOptions>(configuration.GetSection("Users:Inbox"));

		builder.Services.ConfigureOptions<ConfigurePlacesJob>();
		builder.Services.Configure<PlacesSettings>(builder.Configuration);

	}

	private static void AddDomainEventHandlers(this IServiceCollection services)
	{
		var domainEventHandlers = Application.AssemblyReference.Assembly.GetTypes()
											 .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
											 .ToArray();

		foreach (var domainEventHandler in domainEventHandlers)
		{
			services.TryAddScoped(domainEventHandler);

			var domainEvent = domainEventHandler.GetInterfaces()
												.Single(i => i.IsGenericType)
												.GetGenericArguments()
												.Single();

			//var closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

			//services.Decorate(domainEventHandler, closedIdempotentHandler);
		}
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

			//var closedIdempotentHandler = typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

			//services.Decorate(integrationEventHandler, closedIdempotentHandler);
		}
	}
}