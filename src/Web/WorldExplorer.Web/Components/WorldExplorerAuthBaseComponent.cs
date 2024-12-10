namespace WorldExplorer.Web.Components;

using Common.Infrastructure;
using Microsoft.AspNetCore.Components;
using Services.User;

public abstract class WorldExplorerAuthBaseComponent : WorldExplorerBaseComponent
{
	protected UserInfo CurrentUser { get; private set; } = null!;

	[Inject]
	public required ICurrentUserService CurrentUserService { get; set; }

	[Inject]
	public required NavigationManager NavigationManager { get; set; }

	protected override async Task OnInitializedAsync()
	{
		CurrentUser = CurrentUserService.GetCurrentUser();
		if (string.IsNullOrEmpty(CurrentUser.ProviderId))
		{
			Logout();
		}

		await I18NText.SetCurrentLanguageAsync(CurrentUser.Language.GetDescription());
		await base.OnInitializedAsync();
	}

	public void Logout()
	{
		NavigationManager.NavigateTo("MicrosoftIdentity/Account/SignOut", true);
	}
}