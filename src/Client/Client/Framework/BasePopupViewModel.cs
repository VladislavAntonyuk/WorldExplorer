namespace Client.Framework;

public class BasePopupViewModel : BaseViewModel, IQueryAttributable
{
	public const string TaskCompletionSourceKey = "taskCompletionSource";

	private TaskCompletionSource? taskCompletionSource;

	public virtual Task ResetState()
	{
		return Task.CompletedTask;
	}

	public async Task ClosePopup()
	{
		await Shell.Current.GoToAsync("..");
		taskCompletionSource?.SetResult();
	}

	public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		taskCompletionSource = query[TaskCompletionSourceKey] as TaskCompletionSource;
	}
}