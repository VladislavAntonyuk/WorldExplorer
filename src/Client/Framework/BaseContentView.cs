namespace Client.Framework;

public abstract class BaseContentView<T> : ContentView where T : BaseViewModel
{
	protected BaseContentView(T viewModel)
	{
		BindingContext = ViewModel = viewModel;
		Loaded += delegate
		{
			ViewModel.InitializeAsync();
		};
		Unloaded += delegate
		{
			ViewModel.UnInitializeAsync();
		};
	}

	public T ViewModel { get; }
}