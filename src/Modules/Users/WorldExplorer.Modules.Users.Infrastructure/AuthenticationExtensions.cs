namespace WorldExplorer.Modules.Users.Infrastructure;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Presentation.Users;

internal static class AuthenticationExtensions
{
	public const string BasicAuthenticationPolicyName = "BasicAuthenticationPolicy";
	private const string BasicAuthenticationName = "BasicAuthentication";

	public static IServiceCollection AddAuth(this IServiceCollection builder, IConfiguration configuration)
	{
		builder
			.AddAuthN()
			.AddAuthZ();

		return builder;
	}

	private static IServiceCollection AddAuthN(this IServiceCollection builder)
	{
		builder.AddAuthentication()
			   .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationName, null);

		return builder;
	}

	private static IServiceCollection AddAuthZ(this IServiceCollection builder)
	{
		builder.AddAuthorization(options =>
		{
			options.AddPolicy(BasicAuthenticationPolicyName, policyBuilder => policyBuilder.AddAuthenticationSchemes(BasicAuthenticationName).RequireAuthenticatedUser().Build());
		});

		return builder;
	}
}