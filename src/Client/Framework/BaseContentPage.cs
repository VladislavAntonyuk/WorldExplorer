namespace Client.Framework;

using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

public abstract class BaseContentPage<T> : ContentPage where T : BaseViewModel
{
	protected BaseContentPage(T viewModel)
	{
		BindingContext = ViewModel = viewModel;
		Title = viewModel.Title;
		On<iOS>().SetUseSafeArea(true);
#if ANDROID || IOS
		Behaviors.Add(new CommunityToolkit.Maui.Behaviors.StatusBarBehavior
		{
			StatusBarColor = BackgroundColor
		});
#endif
	}

	public T ViewModel { get; }

	protected override void OnAppearing()
	{
		base.OnAppearing();
		ViewModel.InitializeAsync();
	}

	protected override void OnDisappearing()
	{
		ViewModel.UnInitializeAsync();
		base.OnDisappearing();
	}
}