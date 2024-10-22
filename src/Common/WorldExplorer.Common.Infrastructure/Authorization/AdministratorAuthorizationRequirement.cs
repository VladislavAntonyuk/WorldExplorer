namespace WorldExplorer.Common.Infrastructure.Authorization;

public class AdministratorAuthorizationRequirement : RoleAuthorizationRequirement
{
	public override List<string> RequiredRoles => new()
	{
		"Administrator"
	};
}