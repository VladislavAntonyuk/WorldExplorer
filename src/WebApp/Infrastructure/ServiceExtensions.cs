﻿namespace WebApp.Infrastructure;

using global::Shared.Enums;
using global::Shared.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Identity.Web;
using Policies;
using Toolbelt.Blazor.Extensions.DependencyInjection;

public static class Constants
{
	public const string AdministratorPolicy = "IsAdministrator";
}
public static class ServiceExtensions
{
	public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSingleton<IAuthorizationHandler, AdministratorAuthorizationHandler>();
		services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
				.AddMicrosoftIdentityWebApp(options =>
				{
					configuration.Bind("AzureAdB2C", options);
					options.TokenValidationParameters.ValidateIssuer = false;
				});
		services.AddAuthentication().AddMicrosoftIdentityWebApi(configuration, "AzureAdB2C");
		services.AddAuthorization(options =>
		{
			var administratorOrHigherPolicyBuilder =
				new AuthorizationPolicyBuilder().AddAuthenticationSchemes(
					OpenIdConnectDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme);
			administratorOrHigherPolicyBuilder.Requirements.Add(new AdministratorAuthorizationRequirement());
			options.AddPolicy(Constants.AdministratorPolicy, administratorOrHigherPolicyBuilder.Build());
		});
	}

	public static void AddTranslations(this IServiceCollection services)
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