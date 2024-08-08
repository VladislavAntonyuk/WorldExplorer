namespace Client.Framework;

using Extensions;

public abstract class BaseContentView<T> : ContentView where T : BaseViewModel
{
	protected BaseContentView(T viewModel)
	{
		BindingContext = ViewModel = viewModel;
		Loaded += delegate
		{
			InitializeAsync().AndForget(true);
		};
		Unloaded += delegate
		{
			UnInitializeAsync().AndForget(true);
		};
	}

	protected T ViewModel { get; }

	protected virtual Task InitializeAsync()
	{
		ViewModel.InitializeAsync();
		return Task.CompletedTask;
	}

	protected virtual Task UnInitializeAsync()
	{
		ViewModel.UnInitializeAsync().AndForget();
		return Task.CompletedTask;
	}
}