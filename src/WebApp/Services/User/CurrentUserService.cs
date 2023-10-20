namespace WebApp.Services.User;

using System.Security.Claims;
using Microsoft.Identity.Web;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
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