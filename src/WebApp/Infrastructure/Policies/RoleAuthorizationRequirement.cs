namespace WebApp.Infrastructure.Policies;

using Microsoft.AspNetCore.Authorization;

public abstract class RoleAuthorizationRequirement : IAuthorizationRequirement
{
	public abstract List<string> RequiredRoles { get; }
}