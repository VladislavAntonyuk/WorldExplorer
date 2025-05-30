﻿namespace WorldExplorer.Modules.Users.IntegrationTests.Abstractions.Fixtures;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

public class TestAuthHandler(
    IOptionsMonitor<TestAuthHandlerOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<TestAuthHandlerOptions>(options, logger, encoder)
{
    public const string NoPermissionEmail = "NoPermissionUser@email.com";
    public const string Email = "User@email.com";
    private readonly IOptionsMonitor<TestAuthHandlerOptions> options = options;

    public static string AuthenticationScheme { get; } = JwtBearerDefaults.AuthenticationScheme;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        List<Claim> claims =
        [
            new(ClaimConstants.ObjectId, "19d3b2c7-8714-4851-ac73-95aeecfba3a6"),
            new(ClaimConstants.NameIdentifierId, "123123"),
            new(JwtRegisteredClaimNames.Iat, GetEpochTimeFromSeconds().ToString()),
            options.CurrentValue.FailPermission
                ? new(ClaimConstants.PreferredUserName, NoPermissionEmail)
                : new(ClaimConstants.PreferredUserName, Email)
        ];

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);
        var result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }

    public static long GetEpochTimeFromSeconds()
    {
	    return (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
    }
}
