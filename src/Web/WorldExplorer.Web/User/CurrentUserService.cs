namespace WorldExplorer.Web.User;

using System.Security.Claims;
using Microsoft.Identity.Web;
using WorldExplorer.Modules.Users.Application.Abstractions.Identity;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
	public UserInfo GetCurrentUser()
	{
		var user = httpContextAccessor.HttpContext?.User;
		return new UserInfo
		{
			ProviderId = user?.GetObjectId(),
			Name = user?.GetDisplayName() ?? string.Empty,
			Email = user?.FindFirstValue("emails") ?? string.Empty,
			IsNew = Convert.ToBoolean(user?.FindFirstValue("newUser")),
			Language = Enum.Parse<Language>(user?.FindFirstValue("extension_Language") ?? Language.English.ToString()),
			EnableAccessibility = Convert.ToBoolean(user?.FindFirstValue("extension_Accessibility"))
		};
	}
}