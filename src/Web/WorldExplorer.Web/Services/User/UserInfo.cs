namespace WorldExplorer.Web.Services.User;

using Modules.Users.Application.Abstractions.Identity;

public class UserInfo
{
	public string? ProviderId { get; init; }
	public string? Name { get; init; }
	public string? Email { get; init; }
	public bool IsNew { get; init; }
	public Language Language { get; init; }
	public bool EnableAccessibility { get; init; }
}