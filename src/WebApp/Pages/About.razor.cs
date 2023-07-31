namespace WebApp.Pages;

using Microsoft.AspNetCore.Components;

public partial class About : WorldExplorerBaseComponent
{
	[Inject]
	public required NavigationManager NavigationManager { get; set; }

	private void SignIn()
	{
		NavigationManager.NavigateTo("MicrosoftIdentity/Account/SignIn", true);
	}
}