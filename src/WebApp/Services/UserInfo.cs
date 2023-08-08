namespace WebApp.Services;

public class UserInfo
{
	public required string ProviderId { get; set; }
	public required string Name { get; set; }
	public required string Email { get; set; }
	public bool IsNew { get; set; }
}