namespace WebApp.Services;

using System.Security.Claims;
using Microsoft.Identity.Web;

public class CurrentUserService : ICurrentUserService
{
	private readonly IHttpContextAccessor httpContextAccessor;

	public CurrentUserService(IHttpContextAccessor httpContextAccessor)
	{
		this.httpContextAccessor = httpContextAccessor;
	}

	public UserInfo GetCurrentUser()
	{
		var user = httpContextAccessor.HttpContext?.User;
		return new UserInfo
		{
			ProviderId = user?.GetObjectId() ?? string.Empty,
			Name = user?.GetDisplayName() ?? string.Empty,
			Email = user?.FindFirstValue("emails") ?? string.Empty,
			IsNew = Convert.ToBoolean(user?.FindFirstValue("newUser"))
		};
	}
}