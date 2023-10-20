namespace WebApp.Components.Pages.User;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Services;
using Shared.Models;
using WebApp.Services.User;

public partial class Profile : WorldExplorerAuthBaseComponent
{
	private User? currentUser;
	[Inject]
	public required NavigationManager NavigationManager { get; set; }

	[Inject]
	public required IDialogService DialogService { get; set; }

	[Inject]
	public required IUserService UserService { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		currentUser = new User
		{
			Name = CurrentUser.Name,
			Email = CurrentUser.Email,
			Id = CurrentUser.ProviderId
		};

		if (currentUser.Email == string.Empty)
		{
			NavigationManager.NavigateTo("MicrosoftIdentity/Account/SignOut", true);
		}
	}

	private Task DeleteAccount()
	{
		return UserService.DeleteUser(CurrentUser.ProviderId, CancellationToken.None);
	}
}