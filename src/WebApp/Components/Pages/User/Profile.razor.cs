namespace WebApp.Components.Pages.User;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Services.User;
using Shared.Models;

public partial class Profile : WorldExplorerAuthBaseComponent
{
	private User? user;
	[Inject]
	public required IDialogService DialogService { get; set; }

	[Inject]
	public required IUserService UserService { get; set; }

	[Inject]
	public required ISnackbar Snackbar { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		user = await UserService.GetUser(CurrentUser.ProviderId, CancellationToken.None);
	}

	private Task DeleteAccount()
	{
		return UserService.DeleteUser(CurrentUser.ProviderId, CancellationToken.None);
	}

	private async Task SaveChanges()
	{
		if (user is not null)
		{
			await UserService.UpdateUser(user, CancellationToken.None);
			Snackbar.Add(Translation.SavedChanges, Severity.Success);
		}
	}
}