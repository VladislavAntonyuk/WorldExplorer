namespace Client.Framework;

using MvvmHelpers;

public abstract class BaseContentView<T> : ContentView where T : BaseViewModel
{
	protected BaseContentView(T viewModel)
	{
		BindingContext = ViewModel = viewModel;
		Loaded += delegate
		{
			InitializeAsync().SafeFireAndForget();
		};
		Unloaded += delegate
		{
			UnInitializeAsync().SafeFireAndForget();
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
		ViewModel.UnInitializeAsync();
		return Task.CompletedTask;
	}
}