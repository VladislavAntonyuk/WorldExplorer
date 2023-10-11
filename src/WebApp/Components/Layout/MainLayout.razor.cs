namespace WebApp.Components.Layout;

using MudBlazor;

public partial class MainLayout : WorldExplorerBaseLayout
{
	private bool drawerOpen = true;
	private bool isDarkMode = true;
	private MudThemeProvider? mudThemeProvider;
	private bool rightToLeft;

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
}