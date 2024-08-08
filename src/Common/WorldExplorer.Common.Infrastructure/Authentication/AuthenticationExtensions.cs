using Microsoft.Extensions.DependencyInjection;

namespace WorldExplorer.Common.Infrastructure.Authentication;

using Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;

internal static class AuthenticationExtensions
{
    internal static IServiceCollection AddAuthenticationInternal(this IServiceCollection services, IConfiguration configuration)
    {
	    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	            .AddMicrosoftIdentityWebApi(options =>
	            {
		            options.TokenValidationParameters.ValidateIssuer = false;

				}, options =>
	            {
		            configuration.Bind(Microsoft.Identity.Web.Constants.AzureAdB2C, options);
		            options.TokenValidationParameters.ValidateIssuer = false; });
		//services.AddMicrosoftIdentityWebApiAuthentication(configuration, Microsoft.Identity.Web.Constants.AzureAdB2C, JwtBearerDefaults.AuthenticationScheme);
		return services;
    }
}
