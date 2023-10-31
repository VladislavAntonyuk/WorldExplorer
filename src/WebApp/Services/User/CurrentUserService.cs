namespace WebApp.Services.User;

using System.Security.Claims;
using Microsoft.Identity.Web;
using Shared.Enums;

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
			IsNew = Convert.ToBoolean(user?.FindFirstValue("newUser")),
			Language = Enum.Parse<Language>(user?.FindFirstValue("extension_Language") ?? Language.English.ToString()),
			EnableAccessibility = Convert.ToBoolean(user?.FindFirstValue("extension_Accessibility"))
		};
	}
}