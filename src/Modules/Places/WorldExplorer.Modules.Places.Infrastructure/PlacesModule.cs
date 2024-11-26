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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Places;
using Presentation;

public static class PlacesModule
{
	public static IHostApplicationBuilder AddPlacesModule(this IHostApplicationBuilder builder)
	{
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

		builder.Services.AddHttpClient("GoogleImages", client => client.BaseAddress = new Uri("https://serpapi.com"));
		builder.Services.AddSingleton<IImageSearchService, ImageSearchService>();

		builder.Services.ConfigureOptions<ConfigurePlacesJob>();
		builder.Services.Configure<PlacesSettings>(builder.Configuration);
	}
}