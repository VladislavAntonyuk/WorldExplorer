namespace WebApp;
using Microsoft.AspNetCore.Components;
using Services.User;
using Shared.Extensions;

public abstract class WorldExplorerAuthBaseComponent : WorldExplorerBaseComponent
{
	protected UserInfo CurrentUser { get; private set; } = null!;

	[Inject]
	public required ICurrentUserService CurrentUserService { get; set; }

	protected override async Task OnInitializedAsync()
	{
		CurrentUser = CurrentUserService.GetCurrentUser();
		await I18NText.SetCurrentLanguageAsync(CurrentUser.Language.GetDescription());
		await base.OnInitializedAsync();
	}
}