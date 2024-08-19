using WorldExplorer.Common.Infrastructure.Outbox;
using WorldExplorer.Common.Presentation.Endpoints;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WorldExplorer.Modules.Places.Infrastructure;

using System.Configuration;
using Common.Application.Abstractions.Data;
using Common.Infrastructure;
using Database;
using Domain.LocationInfo;
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
	    builder.AddDatabase<PlacesDbContext>(Schemas.Places, options => options.UseNetTopologySuite());

		builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<PlacesDbContext>());

        builder.Services.AddScoped<IPlaceRepository, PlaceRepository>();
        builder.Services.AddScoped<ILocationInfoRepository, LocationInfoRepository>();




		//services.AddHostedService<PlacesLookupBackgroundService>();
		//services.AddHostedService<PlaceDetailsBackgroundService>();

		//services.AddSingleton<IPlacesService, PlacesService>();
		//services.Configure<PlacesSettings>(configuration.GetRequiredSection("Places"));
		//services.AddSingleton<ILocationInfoRequestsService, LocationInfoRepository>();
		if (builder.Configuration.GetValue<string>("AIProvider") == "OpenAI")
		{
			builder.AddAzureOpenAIClient("openai");
			builder.Services.AddSingleton<IAiProvider, OpenAiProvider>();
		}
		else
		{
			builder.Services.Configure<GeminiAiSettings>(builder.Configuration.GetRequiredSection("GeminiAISettings"));
			builder.Services.AddHttpClient<IAiProvider, GeminiProvider>("Gemini", client =>
			{
				client.DefaultRequestHeaders.Add("x-goog-api-client", "genai-swift/0.4.8");
				client.DefaultRequestHeaders.Add("User-Agent", "AIChat/1 CFNetwork/1410.1 Darwin/22.6.0");
			});
		}

		builder.Services.AddSingleton<IAiService, AiService>();

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
