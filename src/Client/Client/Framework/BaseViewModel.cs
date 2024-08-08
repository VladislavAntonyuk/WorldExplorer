namespace Client.Framework;

using CommunityToolkit.Mvvm.ComponentModel;

public abstract partial class BaseViewModel : ObservableObject
{
	[ObservableProperty]
	private string title = string.Empty;

	public virtual Task InitializeAsync()
	{
		return Task.CompletedTask;
	}

	public virtual Task UnInitializeAsync()
	{
		return Task.CompletedTask;
	}
}