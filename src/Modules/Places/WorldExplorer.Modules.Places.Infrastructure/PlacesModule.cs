namespace WorldExplorer.Modules.Places.Infrastructure;

using AI;
using Application.Abstractions;
using Application.Abstractions.Data;
using Common.Application.Messaging;
using Common.Infrastructure;
using Common.Infrastructure.Serialization;
using Common.Presentation.Endpoints;
using Database;
using Domain.LocationInfo;
using Domain.Places;
using Image;
using LocationInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Outbox;
using Places;
using Presentation;

public static class PlacesModule
{
	public static IHostApplicationBuilder AddPlacesModule(this IHostApplicationBuilder builder)
	{
		builder.Services.AddDomainEventHandlers();

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

		if (builder.Configuration.GetValue<string>("Places:AIProvider") == "OpenAI")
		{
			builder.AddOpenAIClient("openai");
			builder.AddOpenAiClient("gpt-4o-mini");
		}
		else
		{
			builder.AddOllamaChatClient("ai-llama3-2");
		}

		builder.Services.AddScoped<IAiService, AiService>();

		builder.Services.Configure<ImageSearchSettings>(builder.Configuration.GetSection("ImageSearch"));
		builder.Services.AddHttpClient("ImageSearch");
		builder.Services.AddSingleton<IImageSearchService, ImageSearchService>();

		builder.Services.ConfigureOptions<ConfigurePlacesJob>();
		builder.Services.Configure<PlacesSettings>(builder.Configuration);

		builder.Services.Configure<OutboxOptions>(builder.Configuration.GetSection("Places:Outbox"));

		builder.Services.ConfigureOptions<ConfigureProcessOutboxJob>();

		SerializerSettings.ConfigureJsonSerializerSettingsInstance([new PointNewtonsoftJsonConverter()]);
		SerializerSettings.ConfigureJsonSerializerOptionsInstance([new PointJsonConverter()]);
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

			var closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

			services.Decorate(domainEventHandler, closedIdempotentHandler);
		}
	}

}