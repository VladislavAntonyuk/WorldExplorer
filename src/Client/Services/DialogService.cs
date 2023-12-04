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
		if (Application.Current?.MainPage is null)
		{
			return Task.CompletedTask;
		}

		return Application.Current.MainPage.DisplayAlert(title, message, cancel);
	}

	public async Task<bool> ConfirmAsync(string title, string message, string ok, string cancel)
	{
		if (Application.Current?.MainPage is null)
		{
			return false;
		}

		var result = await Application.Current.MainPage.DisplayAlert(title, message, ok, cancel);
		return result;
	}
}