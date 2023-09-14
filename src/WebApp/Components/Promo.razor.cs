namespace WebApp.Components;

using Microsoft.AspNetCore.Components;

public partial class Promo : WorldExplorerBaseComponent
{
	[Inject]
	public required NavigationManager NavigationManager { get; set; }

	private void SignIn()
	{
		NavigationManager.NavigateTo("MicrosoftIdentity/Account/SignIn", true);
	}
}