namespace WorldExplorer.Web.Components;

using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using User;

public abstract class WorldExplorerAuthBaseComponent : WorldExplorerBaseComponent
{
	protected UserInfo CurrentUser { get; private set; } = null!;

	[Inject]
	public required AuthenticationStateProvider AuthenticationStateProvider { get; set; }

	[Inject]
	public required IHttpContextAccessor HttpContextAccessor { get; set; }

	[Inject]
	public required ICurrentUserService CurrentUserService { get; set; }

	[Inject]
	public required NavigationManager NavigationManager { get; set; }

	protected override async Task OnInitializedAsync()
	{
		ClaimsPrincipal.ClaimsPrincipalSelector = () =>
		{
			var state = HttpContextAccessor.HttpContext;
			return state.User;
		};
		CurrentUser = CurrentUserService.GetCurrentUser();
		if (CurrentUser.Email == string.Empty)
		{
			Logout();
		}

		//await I18NText.SetCurrentLanguageAsync(CurrentUser.Language.GetDescription());
		await base.OnInitializedAsync();
	}

	public void Logout()
	{
		NavigationManager.NavigateTo("MicrosoftIdentity/Account/SignOut", true);
	}
}