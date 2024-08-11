using WorldExplorer.Common.Infrastructure.Outbox;
using WorldExplorer.Common.Presentation.Endpoints;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WorldExplorer.Modules.Places.Infrastructure;

using System.Configuration;
using Common.Application.Abstractions.Data;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using Places;
using WebApp.Services.AI;
using WebApp.Services.Image;
using WebApp.Services.Place;

public static class PlacesModule
{
    public static IHostApplicationBuilder AddPlacesModule(
        this IHostApplicationBuilder builder)
    {
        builder.Services.AddDomainEventHandlers();

        builder.Services.AddIntegrationEventHandlers();

        builder.AddInfrastructure();

        builder.Services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        return builder;
    }

    private static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
	    builder.Services.AddDbContext<PlacesDbContext>((sp, options) =>
            options
                .UseSqlServer(
                    builder.Configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Places)
                        .UseNetTopologySuite())
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));

        builder.Services.AddScoped<IPlaceRepository, PlaceRepository>();

        builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<PlacesDbContext>());
		builder.AddAzureOpenAIClient("openai");



		//services.AddHostedService<PlacesLookupBackgroundService>();
		//services.AddHostedService<PlaceDetailsBackgroundService>();

		//services.AddSingleton<IPlacesService, PlacesService>();
		//services.Configure<PlacesSettings>(configuration.GetRequiredSection("Places"));
		//services.AddSingleton<ILocationInfoRequestsService, LocationInfoRequestsService>();
		builder.Services.Configure<AiSettings>(builder.Configuration.GetRequiredSection("AI"));
		builder.Services.AddSingleton<IAiService, AiService>();
		builder.Services.AddHttpClient<GeminiProvider>("Gemini", client =>
		{
			client.DefaultRequestHeaders.Add("x-goog-api-client", "genai-swift/0.4.8");
			client.DefaultRequestHeaders.Add("User-Agent", "AIChat/1 CFNetwork/1410.1 Darwin/22.6.0");
		});
		builder.Services.AddSingleton<IAiProvider>(s =>
		{
			var aiSettings = s.GetRequiredService<IOptions<AiSettings>>().Value;
			if (aiSettings.Provider == "OpenAI")
			{
				return new OpenAiProvider(s.GetRequiredService<OpenAIClient>(), s.GetRequiredService<ILogger<OpenAiProvider>>());
			}

			return s.GetRequiredService<GeminiProvider>();
		});

		builder.Services.AddHttpClient("GoogleImages", client => client.BaseAddress = new Uri("https://serpapi.com"));
		builder.Services.AddSingleton<IImageSearchService, ImageSearchService>();



		//services.Configure<OutboxOptions>(configuration.GetSection("Users:Outbox"));

		//services.ConfigureOptions<ConfigureProcessOutboxJob>();

		//services.Configure<InboxOptions>(configuration.GetSection("Users:Inbox"));

		//services.ConfigureOptions<ConfigureProcessInboxJob>();
	}

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        //Type[] domainEventHandlers = Application.AssemblyReference.Assembly
        //    .GetTypes()
        //    .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
        //    .ToArray();

        //foreach (Type domainEventHandler in domainEventHandlers)
        //{
        //    services.TryAddScoped(domainEventHandler);

        //    Type domainEvent = domainEventHandler
        //        .GetInterfaces()
        //        .Single(i => i.IsGenericType)
        //        .GetGenericArguments()
        //        .Single();

        //    Type closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

        //    //services.Decorate(domainEventHandler, closedIdempotentHandler);
       // }
    }

    private static void AddIntegrationEventHandlers(this IServiceCollection services)
    {
       // Type[] integrationEventHandlers = Presentation.AssemblyReference.Assembly
        //    .GetTypes()
        //    .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
        //    .ToArray();

        //foreach (Type integrationEventHandler in integrationEventHandlers)
        //{
        //    services.TryAddScoped(integrationEventHandler);

        //    Type integrationEvent = integrationEventHandler
        //        .GetInterfaces()
        //        .Single(i => i.IsGenericType)
        //        .GetGenericArguments()
        //        .Single();

        //    Type closedIdempotentHandler =
        //        typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

        //    //services.Decorate(integrationEventHandler, closedIdempotentHandler);
        //}
    }
}
