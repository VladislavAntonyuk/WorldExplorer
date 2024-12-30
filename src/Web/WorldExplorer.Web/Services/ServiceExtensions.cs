namespace WorldExplorer.Web.Services;

using Common.Infrastructure;
using Microsoft.AspNetCore.Localization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Modules.Users.Application.Abstractions.Identity;
using MudBlazor.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Toolbelt.Blazor.I18nText;
using User;
using Constants = Microsoft.Identity.Web.Constants;
using MicrosoftIdentityUserAuthenticationMessageHandler = MicrosoftIdentityUserAuthenticationMessageHandler;

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
		var scopes = configuration.GetRequiredSection("WorldExplorerApiClient:Scopes").Get<string>()?.Split(' ');
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
		var baseUrl = configuration.GetRequiredSection("WorldExplorerApiClient:BaseUrl").Get<Uri>();
		services.AddOptions<MicrosoftIdentityAuthenticationMessageHandlerOptions>()
				.Bind(configuration.GetSection("WorldExplorerApiClient"));
		services.AddTransient<MicrosoftIdentityUserAuthenticationMessageHandler>();
		services.AddHttpClient<WorldExplorerApiClient>(httpClient =>
				{
					httpClient.BaseAddress = baseUrl;
				})
				.AddHttpMessageHandler<MicrosoftIdentityUserAuthenticationMessageHandler>();

		services.AddWorldExplorerTravellersClient()
				.ConfigureHttpClient(
					client => client.BaseAddress = new Uri($"{baseUrl}graphql"),
					builder => builder.AddHttpMessageHandler<MicrosoftIdentityUserAuthenticationMessageHandler>());
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