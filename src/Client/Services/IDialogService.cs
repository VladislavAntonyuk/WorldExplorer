namespace Client.Services;

public interface IDialogService
{
	Task<bool> ConfirmAsync(string title, string message, string ok, string cancel);

	Task SnackBarAsync(string message,
		string action,
		Action? actionHandler,
		CancellationToken cancellationToken = default);

	Task ToastAsync(string message, CancellationToken cancellationToken = default);

	Task ToastAsync(IEnumerable<string> messages, CancellationToken cancellationToken = default)
	{
		return ToastAsync(string.Join(Environment.NewLine, messages), cancellationToken);
	}
}