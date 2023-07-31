namespace WebApp.Services;

using System.Security.Claims;
using Microsoft.Identity.Web;

public interface ICurrentUserService
{
	UserInfo GetCurrentUser();
}

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

public class UserInfo
{
	public required string ProviderId { get; set; }
	public required string Name { get; set; }
	public required string Email { get; set; }
	public bool IsNew { get; set; }
}