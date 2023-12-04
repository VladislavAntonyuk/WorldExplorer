namespace WebApp.Components;
using Microsoft.AspNetCore.Components;
using Services.User;
using Shared.Extensions;

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
		if (CurrentUser.Email == string.Empty)
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