namespace WebApp.Infrastructure.Policies;

public class AdministratorAuthorizationRequirement : RoleAuthorizationRequirement
{
	public override List<string> RequiredRoles => new()
	{
		"Administrator"
	};
}