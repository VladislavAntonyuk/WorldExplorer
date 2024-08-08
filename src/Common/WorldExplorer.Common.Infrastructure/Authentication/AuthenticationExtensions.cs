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
	    services.AddMicrosoftIdentityWebApiAuthentication(configuration, Microsoft.Identity.Web.Constants.AzureAdB2C);

		return services;
    }
}
