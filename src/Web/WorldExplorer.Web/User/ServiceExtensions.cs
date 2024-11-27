namespace WorldExplorer.Web.User;

using Common.Infrastructure;
using Microsoft.AspNetCore.Localization;
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
		var scopes = configuration.GetSection("WorldExplorerApiClient:Scopes").Get<string>().Split(' ');
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
		var baseUrl = configuration.GetSection("WorldExplorerApiClient:BaseUrl").Get<string>();
		services.AddOptions<MicrosoftIdentityAuthenticationMessageHandlerOptions>()
		        .Bind(configuration.GetSection("WorldExplorerApiClient"));
		services.AddTransient<Web.MicrosoftIdentityUserAuthenticationMessageHandler>();
		services.AddHttpClient<WorldExplorerApiClient>(s =>
		        {
			        s.BaseAddress = new Uri(baseUrl);
		        })
				.AddHttpMessageHandler<Web.MicrosoftIdentityUserAuthenticationMessageHandler>();

		services.AddWorldExplorerTravellersClient()
		        .ConfigureHttpClient(client => client.BaseAddress = new Uri($"{baseUrl}/graphql"));
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