﻿namespace WebApp.Infrastructure;

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
using Toolbelt.Blazor.Extensions.DependencyInjection;

public static class Constants
{
	public const string AdministratorPolicy = "IsAdministrator";
}
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
		services.AddAuth(configuration);
		services.AddCascadingAuthenticationState();
		services.AddHttpContextAccessor();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<ICurrentUserService, CurrentUserService>();
		services.AddSingleton<IGraphClientService>(_ =>
		{
			var config = configuration.GetRequiredSection(AzureAdB2CGraphClientConfiguration.ConfigurationName)
								.Get<AzureAdB2CGraphClientConfiguration>();
			ArgumentNullException.ThrowIfNull(config);
			var clientSecretCredential = new ClientSecretCredential(config.TenantId, config.ClientId, config.ClientSecret);
			return new GraphClientService(new GraphServiceClient(clientSecretCredential), config.DefaultApplicationId);
		});
	}

	public static void AddDatabase(this IServiceCollection services)
	{
		Directory.CreateDirectory("App_Data");
		services.AddPooledDbContextFactory<WorldExplorerDbContext>(opt =>
		{
			opt.UseSqlite("Data Source=App_Data/WorldExplorer.db", optionsBuilder => optionsBuilder.UseNetTopologySuite())
			   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
		});
	}

	public static void AddWorldExplorerServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddHostedService<PlacesLookupBackgroundService>();
		services.AddHostedService<PlaceDetailsBackgroundService>();

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

	private static void AddAuth(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSingleton<IAuthorizationHandler, AdministratorAuthorizationHandler>();
		services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
				.AddMicrosoftIdentityWebApp(options =>
				{
					configuration.Bind(Microsoft.Identity.Web.Constants.AzureAdB2C, options);
					options.TokenValidationParameters.ValidateIssuer = false;
				});
		services.AddAuthentication()
				.AddMicrosoftIdentityWebApi(options =>
				{
					options.TokenValidationParameters.ValidateIssuer = false;
				}, options =>
				{
					configuration.Bind(Microsoft.Identity.Web.Constants.AzureAdB2C, options);
					options.TokenValidationParameters.ValidateIssuer = false;
				});
		services.AddAuthorization(options =>
		{
			var administratorOrHigherPolicyBuilder =
				new AuthorizationPolicyBuilder().AddAuthenticationSchemes(
					OpenIdConnectDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme);
			administratorOrHigherPolicyBuilder.Requirements.Add(new AdministratorAuthorizationRequirement());
			options.AddPolicy(Constants.AdministratorPolicy, administratorOrHigherPolicyBuilder.Build());
		});
	}

	private static void AddTranslations(this IServiceCollection services)
	{
		services.AddI18nText();
		services.Configure<RequestLocalizationOptions>(options =>
		{
			var supportedCultures = Enum.GetValues<Language>().Select(x => x.GetDescription()).ToArray();
			options.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
			options.AddSupportedCultures(supportedCultures);
			options.AddSupportedUICultures(supportedCultures);
		});
	}
}