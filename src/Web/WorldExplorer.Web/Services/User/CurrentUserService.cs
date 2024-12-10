namespace WorldExplorer.Web.Services.User;

using System.Security.Claims;
using Microsoft.Identity.Web;
using Modules.Users.Application.Abstractions.Identity;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
	public UserInfo GetCurrentUser()
	{
		var user = httpContextAccessor.HttpContext?.User;
		if (user is null)
		{
			return new UserInfo();
		}

		return new UserInfo
		{
			ProviderId = user.GetObjectId(),
			Name = user.GetDisplayName(),
			Email = user.FindFirstValue("emails"),
			IsNew = Convert.ToBoolean(user.FindFirstValue("newUser")),
			Language = Enum.Parse<Language>(user.FindFirstValue("extension_Language") ?? nameof(Language.English)),
			EnableAccessibility = Convert.ToBoolean(user.FindFirstValue("extension_Accessibility"))
		};
	}
}