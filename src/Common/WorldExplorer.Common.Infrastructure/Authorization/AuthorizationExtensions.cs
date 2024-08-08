using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace WorldExplorer.Common.Infrastructure.Authorization;

using Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

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
			var administratorOrHigherPolicyBuilder = new AuthorizationPolicyBuilder().AddAuthenticationSchemes(
					OpenIdConnectDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme);
			administratorOrHigherPolicyBuilder.Requirements.Add(new AdministratorAuthorizationRequirement());
			options.AddPolicy(Constants.AdministratorPolicy, administratorOrHigherPolicyBuilder.Build());
		});

        return services;
    }
}
