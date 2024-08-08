namespace WorldExplorer.Web.Components.Pages.User;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using WorldExplorer.Modules.Users.Application.Users.GetUser;
using WorldExplorer.Web.Components;

public partial class Profile(IDialogService dialogService, WorldExplorerApiClient apiClient, ISnackbar snackbar) : WorldExplorerAuthBaseComponent
{
	private UserResponse? user;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		user = await apiClient.GetCurrentUser(CancellationToken.None);
	}

	private Task DeleteAccount()
	{
		return apiClient.DeleteUser(CurrentUser.ProviderId, CancellationToken.None);
	}

	private async Task SaveChanges()
	{
		if (user is not null)
		{
			await apiClient.UpdateUser(user, CancellationToken.None);
			snackbar.Add("Translation.SavedChanges", Severity.Success);
		}
	}
}