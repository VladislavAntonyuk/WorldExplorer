namespace Client.Controls;

using System.Collections.ObjectModel;

public class ArView : View, IArView
{
	public static readonly BindableProperty ImagesProperty = BindableProperty.Create(
		nameof(Images), typeof(ObservableCollection<string>), typeof(ArView),
		defaultValueCreator: _ => new ObservableCollection<string>(), defaultBindingMode: BindingMode.TwoWay);

	public ObservableCollection<string> Images
	{
		get => (ObservableCollection<string>)GetValue(ImagesProperty);
		set => SetValue(ImagesProperty, value);
	}
}