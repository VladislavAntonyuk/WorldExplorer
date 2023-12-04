namespace WebApp.Components.Pages.User;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Services.User;
using Shared.Models;
using WebApp.Components;

public partial class Profile : WorldExplorerAuthBaseComponent
{
	private User? user;
	[Inject]
	public required IDialogService DialogService { get; set; }

	[Inject]
	public required IUserService UserService { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		user = await UserService.GetUser(CurrentUser.ProviderId, CancellationToken.None);
	}

	private Task DeleteAccount()
	{
		return UserService.DeleteUser(CurrentUser.ProviderId, CancellationToken.None);
	}

	private Task SaveChanges()
	{
		return user is null ? Task.CompletedTask : UserService.UpdateUser(user, CancellationToken.None);
	}
}