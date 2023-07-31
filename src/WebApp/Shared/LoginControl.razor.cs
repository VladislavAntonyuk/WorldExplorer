namespace WebApp.Shared;

using Microsoft.AspNetCore.Components;

public partial class LoginControl : WorldExplorerAuthBaseComponent
{
	[Inject]
	public required NavigationManager NavigationManager { get; set; }

	private void Logout()
	{
		NavigationManager.NavigateTo("MicrosoftIdentity/Account/SignOut", true);
	}
}