namespace WorldExplorer.Web.Components.Layout;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

public partial class MainLayout : WorldExplorerBaseLayout
{
	private bool drawerOpen = true;
	private bool isDarkMode = true;
	private MudThemeProvider? mudThemeProvider;
	private bool rightToLeft;
	private ErrorBoundary? errorBoundary;

	[Inject]
	public required NavigationManager NavigationManager { get; set; }

	private void DrawerToggle()
	{
		drawerOpen = !drawerOpen;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender && mudThemeProvider is not null)
		{
			var isDarkSystemPreference = await mudThemeProvider.GetSystemPreference();
			await OnSystemPreferenceChanged(isDarkSystemPreference);
			await mudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
		}
	}

	private Task OnSystemPreferenceChanged(bool isDarkSystemPreference)
	{
		if (isDarkMode != isDarkSystemPreference)
		{
			isDarkMode = isDarkSystemPreference;
			StateHasChanged();
		}

		return Task.CompletedTask;
	}
	private void ResetError()
	{
		errorBoundary?.Recover();
		NavigationManager.NavigateTo("/");
	}
}