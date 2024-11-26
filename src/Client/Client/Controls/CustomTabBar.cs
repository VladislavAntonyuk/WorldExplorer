namespace Client.Controls;

using System.Windows.Input;

public class CustomTabBar : TabBar
{
	public static readonly BindableProperty CenterViewVisibleProperty = BindableProperty.Create(nameof(CenterViewVisible), typeof(bool), typeof(CustomTabBar), default(bool));
	public static readonly BindableProperty CenterViewImageSourceProperty = BindableProperty.Create(nameof(CenterViewImageSource), typeof(ImageSource), typeof(CustomTabBar));
	public static readonly BindableProperty CenterViewBackgroundColorProperty = BindableProperty.Create(nameof(CenterViewBackgroundColor), typeof(Color), typeof(CustomTabBar), Colors.Transparent);
	public static readonly BindableProperty CenterViewCommandProperty = BindableProperty.Create(nameof(CenterViewCommand), typeof(ICommand), typeof(CustomTabBar));
	
	public ICommand? CenterViewCommand
	{
		get => (ICommand?)GetValue(CenterViewCommandProperty);
		set => SetValue(CenterViewCommandProperty, value);
	}

	public Color CenterViewBackgroundColor
	{
		get => (Color)GetValue(CenterViewBackgroundColorProperty);
		set => SetValue(CenterViewBackgroundColorProperty, value);
	}

	public ImageSource? CenterViewImageSource
	{
		get => (ImageSource?)GetValue(CenterViewImageSourceProperty);
		set => SetValue(CenterViewImageSourceProperty, value);
	}

	public bool CenterViewVisible
	{
		get => (bool)GetValue(CenterViewVisibleProperty);
		set => SetValue(CenterViewVisibleProperty, value);
	}
}