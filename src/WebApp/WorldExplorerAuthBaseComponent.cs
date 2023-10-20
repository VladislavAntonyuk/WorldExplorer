namespace WebApp;

using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Web;
using Services;
using WebApp.Services.User;

public abstract class WorldExplorerAuthBaseComponent : WorldExplorerBaseComponent
{
	protected UserInfo CurrentUser { get; private set; } = null!;

	[CascadingParameter]
	public required Task<AuthenticationState> AuthenticationState { get; set; }

	protected override async Task OnInitializedAsync()
	{
		var authenticationStateUser = (await AuthenticationState).User;

		CurrentUser = new UserInfo
		{
			ProviderId = authenticationStateUser.GetObjectId() ?? string.Empty,
			Name = authenticationStateUser.GetDisplayName() ?? string.Empty,
			Email = authenticationStateUser.FindFirstValue("emails") ?? string.Empty,
			IsNew = Convert.ToBoolean(authenticationStateUser.FindFirstValue("newUser"))
		};
		await I18NText.SetCurrentLanguageAsync(authenticationStateUser.FindFirstValue("extension_Language") ?? "en-US");
		await base.OnInitializedAsync();
	}
}