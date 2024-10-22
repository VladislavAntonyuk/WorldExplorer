namespace Client.Services;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

internal class DialogService : IDialogService
{
	public Task SnackBarAsync(string message,
		string action,
		Action? actionHandler,
		CancellationToken cancellationToken = default)
	{
		return Snackbar.Make(message, actionHandler, action).Show(cancellationToken);
	}

	public Task ToastAsync(string message, CancellationToken cancellationToken = default)
	{
		return Toast.Make(message, ToastDuration.Long).Show(cancellationToken);
	}

	public Task AlertAsync(string title, string message, string cancel)
	{
		var activePage = GetActivePage();
		if (activePage is null)
		{
			return Task.CompletedTask;
		}

		return activePage.DisplayAlert(title, message, cancel);
	}

	public async Task<bool> ConfirmAsync(string title, string message, string ok, string cancel)
	{
		var activePage = GetActivePage();
		if (activePage is null)
		{
			return false;
		}

		var result = await activePage.DisplayAlert(title, message, ok, cancel);
		return result;
	}

	private static Page? GetActivePage()
	{
		return Application.Current?.Windows.LastOrDefault()?.Page;
	}
}