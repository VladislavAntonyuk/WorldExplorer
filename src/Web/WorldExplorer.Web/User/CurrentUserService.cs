﻿namespace WorldExplorer.Modules.Users.Infrastructure.User;

using System.Security.Claims;
using Application.Abstractions.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;

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