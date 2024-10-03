namespace WorldExplorer.Modules.Places.Infrastructure;

using System.ClientModel;
using AI;
using Application.Abstractions;
using Application.Abstractions.Data;
using Common.Infrastructure;
using Common.Infrastructure.Configuration;
using Common.Presentation.Endpoints;
using Database;
using Domain.LocationInfo;
using Domain.Places;
using Image;
using LocationInfo;
using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Ollama;
using OpenAI;
using Places;
using Presentation;
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
		builder.AddDatabase<PlacesDbContext>(Schemas.Places, options => options.UseNetTopologySuite());

		builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<PlacesDbContext>());

		builder.Services.AddScoped<IPlaceRepository, PlaceRepository>();
		builder.Services.AddScoped<ILocationInfoRepository, LocationInfoRepository>();


		//services.AddHostedService<PlacesLookupBackgroundService>();
		//services.AddHostedService<PlaceDetailsBackgroundService>();

		//services.AddSingleton<IPlacesService, PlacesService>();
		//services.Configure<PlacesSettings>(configuration.GetRequiredSection("Places"));
		//services.AddSingleton<ILocationInfoRequestsService, LocationInfoRepository>();
		var aiProvider = builder.Configuration.GetValue<string>("AIProvider");
		if (aiProvider == "OpenAI")
		{
			builder.AddAzureOpenAIClient("openai");
			builder.Services.AddSingleton<IAiProvider, OpenAiProvider>();
		}
		else if (aiProvider == "Ollama")
		{
			builder.Services.AddSingleton<IOllamaApiClient, OllamaApiClient>(provider =>
			{
				var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient();
				var ollamaBaseUrl = builder.Configuration.GetConnectionStringOrThrow("ai");
				return new OllamaApiClient(httpClient, new Uri(ollamaBaseUrl));
			});
			builder.Services.AddSingleton<IAiProvider, OllamaProvider>();
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

		builder.Services.ConfigureOptions<ConfigurePlacesJob>();
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