namespace WorldExplorer.Common.Infrastructure.Authorization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

public static class Constants
{
	public const string AdministratorPolicy = "IsAdministrator";
}

internal static class AuthorizationExtensions
{
	internal static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
	{
		services.AddSingleton<IAuthorizationHandler, AdministratorAuthorizationHandler>();
		services.AddAuthorization(options =>
		{
			var administratorOrHigherPolicyBuilder = new AuthorizationPolicyBuilder().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
			administratorOrHigherPolicyBuilder.Requirements.Add(new AdministratorAuthorizationRequirement());
			options.AddPolicy(Constants.AdministratorPolicy, administratorOrHigherPolicyBuilder.Build());
		});

		return services;
	}

	internal static IServiceCollection AddAuthorizationInternal2(this IServiceCollection services)
	{
		services.AddSingleton<IAuthorizationHandler, AdministratorAuthorizationHandler>();
		services.AddAuthorization(options =>
		{
			var administratorOrHigherPolicyBuilder = new AuthorizationPolicyBuilder().AddAuthenticationSchemes(OpenIdConnectDefaults.AuthenticationScheme);
			administratorOrHigherPolicyBuilder.Requirements.Add(new AdministratorAuthorizationRequirement());
			options.AddPolicy(Constants.AdministratorPolicy, administratorOrHigherPolicyBuilder.Build());
		});

		return services;
	}
}