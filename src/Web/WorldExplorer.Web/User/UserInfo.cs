namespace WorldExplorer.Web.User;

using Modules.Users.Application.Abstractions.Identity;

public class UserInfo
{
	public required string ProviderId { get; set; }
	public required string Name { get; set; }
	public required string Email { get; set; }
	public bool IsNew { get; set; }
	public Language Language { get; set; }
	public bool EnableAccessibility { get; set; }
}