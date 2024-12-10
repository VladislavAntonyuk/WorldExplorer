namespace WorldExplorer.Web.Components.Pages.User;

using Modules.Users.Application.Users.GetUser;
using MudBlazor;
using Services;

public partial class Profile(WorldExplorerApiClient apiClient, ISnackbar snackbar) : WorldExplorerAuthBaseComponent
{
	private UserResponse? user;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		user = await apiClient.GetCurrentUser(CancellationToken.None);
	}

	private Task DeleteAccount()
	{
		return apiClient.DeleteUser(CancellationToken.None);
	}

	private async Task SaveChanges()
	{
		if (user is not null)
		{
			await apiClient.UpdateUser(user.Settings.TrackUserLocation, CancellationToken.None);
			snackbar.Add(Translation.SavedChanges, Severity.Success);
		}
	}
}