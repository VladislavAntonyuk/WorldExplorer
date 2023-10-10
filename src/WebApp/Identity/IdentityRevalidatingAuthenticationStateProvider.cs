namespace WebApp.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Services;

public class IdentityRevalidatingAuthenticationStateProvider(ILoggerFactory loggerFactory,
	IServiceScopeFactory scopeFactory,
	IOptions<IdentityOptions> optionsAccessor) : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
	private readonly IdentityOptions options = optionsAccessor.Value;

	protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

	protected override async Task<bool> ValidateAuthenticationStateAsync(
		AuthenticationState authenticationState, CancellationToken cancellationToken)
	{
		// Get the user manager from a new scope to ensure it fetches fresh data
		await using var scope = scopeFactory.CreateAsyncScope();
		var userManager = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
		return await ValidateSecurityStampAsync(userManager, authenticationState.User);
	}

	private async Task<bool> ValidateSecurityStampAsync(ICurrentUserService userManager, ClaimsPrincipal principal)
	{
		await Task.CompletedTask;
		var principalStamp = principal.FindFirstValue(options.ClaimsIdentity.SecurityStampClaimType);
		return userManager.GetCurrentUser().ProviderId != principalStamp;
	}
}
