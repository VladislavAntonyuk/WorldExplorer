namespace Client.Framework;

using CommunityToolkit.Mvvm.Input;
using Services.Navigation;

public abstract partial class BasePopupViewModel(INavigationService navigationService) : BaseViewModel, IQueryAttributable
{
	public const string TaskCompletionSourceKey = "taskCompletionSource";

	private TaskCompletionSource? taskCompletionSource;

	public virtual Task ResetState()
	{
		return Task.CompletedTask;
	}

	[RelayCommand]
	public async Task ClosePopup()
	{
		await navigationService.NavigateBackAsync();
		taskCompletionSource?.SetResult();
	}

	public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		taskCompletionSource = query[TaskCompletionSourceKey] as TaskCompletionSource;
	}
}