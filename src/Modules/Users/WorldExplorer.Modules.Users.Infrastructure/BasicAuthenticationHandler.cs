namespace WorldExplorer.Modules.Users.Presentation.Users;

using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class BasicAuthenticationHandler(
	IOptionsMonitor<AuthenticationSchemeOptions> options,
	ILoggerFactory logger,
	UrlEncoder encoder,
	IConfiguration configuration)
	: AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var authorizationHeader = Request.Headers["Authorization"].ToString();
		if (authorizationHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
		{
			var userName = configuration.GetValue<string>("AzureAdB2CClaimsBasicAuthUsername");
			var password = configuration.GetValue<string>("AzureAdB2CClaimsBasicAuthPassword");
			var token = authorizationHeader["Basic ".Length..].Trim();
			var credentialsAsEncodedString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
			var receivedCredentials = credentialsAsEncodedString.Split(':', 2);
			if (receivedCredentials.Length == 2 && receivedCredentials[0] == userName && receivedCredentials[1] == password)
			{
				var claims = new[] { new Claim("username", receivedCredentials[0]) };
				var identity = new ClaimsIdentity(claims, "Basic");
				var claimsPrincipal = new ClaimsPrincipal(identity);
				return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
			}

			Logger.LogInformation("{Username} failed to log in", receivedCredentials[0]);
		}

		Response.StatusCode = (int)HttpStatusCode.Unauthorized;
		return await Task.FromResult(AuthenticateResult.Fail("Authentication Failed"));
	}
}