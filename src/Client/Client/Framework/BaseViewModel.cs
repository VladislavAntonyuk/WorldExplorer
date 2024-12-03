namespace Client.Framework;

using CommunityToolkit.Mvvm.ComponentModel;

public abstract partial class BaseViewModel : ObservableObject
{
	[ObservableProperty]
	public partial string Title { get; protected set; } = string.Empty;

	public virtual Task InitializeAsync()
	{
		return Task.CompletedTask;
	}

	public virtual Task UnInitializeAsync()
	{
		return Task.CompletedTask;
	}
}