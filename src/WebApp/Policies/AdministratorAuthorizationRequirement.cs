namespace WebApp.Policies;

public class AdministratorAuthorizationRequirement : RoleAuthorizationRequirement
{
	public override List<string> RequiredRoles => new()
	{
		"Administrator"
	};
}