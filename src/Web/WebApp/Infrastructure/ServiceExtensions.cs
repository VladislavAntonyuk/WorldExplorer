namespace WebApp.Infrastructure;

using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Beta;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using Policies;
using Services.AI;
using Services.Image;
using Services.Place;
using Services.User;
using Shared.Enums;
using Shared.Extensions;

public static class ServiceExtensions
{
	public static void AddBlazor(this IServiceCollection services)
	{
		services.AddRazorPages();
		services.AddControllersWithViews().AddMicrosoftIdentityUI();
		services.AddRazorComponents().AddInteractiveServerComponents();
		services.AddMudServices();
		services.AddTranslations();
		services.AddLogging();
	}

	public static void AddUserServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddCascadingAuthenticationState();
		services.AddHttpContextAccessor();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<ICurrentUserService, CurrentUserService>();

	}

	public static void AddDatabase(this IServiceCollection services)
	{
		Directory.CreateDirectory("App_Data");
		services.AddPooledDbContextFactory<WorldExplorerDbContext>(opt =>
		{
			opt.UseSqlServer("Data Source=App_Data/WorldExplorer.db", optionsBuilder => optionsBuilder.UseNetTopologySuite())
			   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
		});
	}

	public static void AddWorldExplorerServices(this IServiceCollection services, IConfiguration configuration)
	{
		//services.AddHostedService<PlacesLookupBackgroundService>();
		//services.AddHostedService<PlaceDetailsBackgroundService>();

		services.AddSingleton<IPlacesService, PlacesService>();
		services.Configure<PlacesSettings>(configuration.GetRequiredSection("Places"));
		services.AddSingleton<ILocationInfoRequestsService, LocationInfoRequestsService>();
		services.Configure<AiSettings>(configuration.GetRequiredSection("AI"));
		services.AddSingleton<IAiService, AiService>();
		services.AddHttpClient<GeminiProvider>("Gemini", client =>
		{
			client.DefaultRequestHeaders.Add("x-goog-api-client", "genai-swift/0.4.8");
			client.DefaultRequestHeaders.Add("User-Agent", "AIChat/1 CFNetwork/1410.1 Darwin/22.6.0");
		});
		services.AddSingleton<IAiProvider>(s =>
		{
			var aiSettings = s.GetRequiredService<IOptions<AiSettings>>().Value;
			if (aiSettings.Provider == "OpenAI")
			{
				return new OpenAiProvider(s.GetRequiredService<IOptions<AiSettings>>(), s.GetRequiredService<ILogger<OpenAiProvider>>());
			}

			return s.GetRequiredService<GeminiProvider>();
		});

		services.AddHttpClient("GoogleImages", client => client.BaseAddress = new Uri("https://serpapi.com"));
		services.AddSingleton<IImageSearchService, ImageSearchService>();
	}

	private static void AddTranslations(this IServiceCollection services)
	{
		//services.AddI18nText();
		services.Configure<RequestLocalizationOptions>(options =>
		{
			var supportedCultures = Enum.GetValues<Language>().Select(x => x.GetDescription()).ToArray();
			options.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
			options.AddSupportedCultures(supportedCultures);
			options.AddSupportedUICultures(supportedCultures);
		});
	}
}