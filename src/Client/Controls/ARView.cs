namespace Client.Controls;

using System.Collections.ObjectModel;

public class ArView : View, IArView
{
	public static readonly BindableProperty ImagesProperty = BindableProperty.Create(
		nameof(Images), typeof(ObservableCollection<byte[]>), typeof(ArView),
		defaultValueCreator: _ => new ObservableCollection<byte[]>(), defaultBindingMode: BindingMode.TwoWay);

	public ObservableCollection<byte[]> Images
	{
		get => (ObservableCollection<byte[]>)GetValue(ImagesProperty);
		set => SetValue(ImagesProperty, value);
	}
}