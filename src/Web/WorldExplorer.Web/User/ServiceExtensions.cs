namespace WorldExplorer.Web.User;

using Common.Infrastructure;
using Microsoft.AspNetCore.Localization;
using Microsoft.Identity.Client.Kerberos;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Modules.Users.Application.Abstractions.Identity;
using MudBlazor.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Toolbelt.Blazor.I18nText;
using Constants = Microsoft.Identity.Web.Constants;

public static class ServiceExtensions
{
	public static void AddBlazor(this IServiceCollection services)
	{
		services.AddRazorPages();
		services.AddControllersWithViews().AddMicrosoftIdentityUI();
		services.AddMudServices();
		services.AddTranslations();
		services.AddLogging();
		services.AddRazorComponents().AddInteractiveServerComponents().AddMicrosoftIdentityConsentHandler();
	}

	public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
	{
		var scopes = configuration.GetSection("WorldExplorerApiClient:Scopes").Get<string[]>();
		services.AddMicrosoftIdentityWebAppAuthentication(configuration, Constants.AzureAdB2C)
		        .EnableTokenAcquisitionToCallDownstreamApi(scopes)
		        .AddDistributedTokenCaches();

		services.AddCascadingAuthenticationState();
		services.AddAuthZ();
	}

	public static void AddUserServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddHttpContextAccessor();
		services.AddScoped<ICurrentUserService, CurrentUserService>();
	}

	public static void AddWorldExplorerServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddOptions<MicrosoftIdentityAuthenticationMessageHandlerOptions>()
		        .Bind(configuration.GetSection("WorldExplorerApiClient"));
		services.AddTransient<WorldExplorer.Web.MicrosoftIdentityUserAuthenticationMessageHandler>();
		services.AddHttpClient<WorldExplorerApiClient>(s =>
		        {
			        s.BaseAddress = new Uri("https+http://apiservice");
		        })
				.AddHttpMessageHandler<Web.MicrosoftIdentityUserAuthenticationMessageHandler>();

		var baseUrl = configuration.GetSection("WorldExplorerApiClient:BaseUrl").Get<string>();
		services.AddWorldExplorerTravellersClient()
		        .ConfigureHttpClient(client => client.BaseAddress = new Uri($"{baseUrl}/graphql"));


		//services.AddHostedService<PlacesLookupBackgroundService>();
		//services.AddHostedService<PlaceDetailsBackgroundService>();

		//services.AddSingleton<IPlacesService, PlacesService>();
		//services.Configure<PlacesSettings>(configuration.GetRequiredSection("Places"));
		//services.AddSingleton<ILocationInfoRequestsService, LocationInfoRequestsService>();
		//services.Configure<AiSettings>(configuration.GetRequiredSection("AI"));
		//services.AddSingleton<IAiService, AiService>();
		//services.AddHttpClient<GeminiProvider>("Gemini", client =>
		//{
		//	client.DefaultRequestHeaders.Add("x-goog-api-client", "genai-swift/0.4.8");
		//	client.DefaultRequestHeaders.Add("User-Agent", "AIChat/1 CFNetwork/1410.1 Darwin/22.6.0");
		//});
		//services.AddSingleton<IAiProvider>(s =>
		//{
		//	var aiSettings = s.GetRequiredService<IOptions<AiSettings>>().Value;
		//	if (aiSettings.Provider == "OpenAI")
		//	{
		//		return new OpenAiProvider(s.GetRequiredService<IOptions<AiSettings>>(), s.GetRequiredService<ILogger<OpenAiProvider>>());
		//	}

		//	return s.GetRequiredService<GeminiProvider>();
		//});

		//services.AddHttpClient("GoogleImages", client => client.BaseAddress = new Uri("https://serpapi.com"));
		//services.AddSingleton<IImageSearchService, ImageSearchService>();
	}

	private static void AddTranslations(this IServiceCollection services)
	{
		services.AddI18nText(options => options.PersistenceLevel = PersistanceLevel.Cookie);
		services.Configure<RequestLocalizationOptions>(options =>
		{
			var supportedCultures = Enum.GetValues<Language>().Select(x => x.GetDescription()).ToArray();
			options.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
			options.AddSupportedCultures(supportedCultures);
			options.AddSupportedUICultures(supportedCultures);
		});
	}
}