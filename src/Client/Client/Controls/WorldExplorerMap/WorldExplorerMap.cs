namespace Client.Controls.WorldExplorerMap;

using System.Collections.ObjectModel;

public class WorldExplorerMap : View, IWorldExplorerMap

{
	public static readonly BindableProperty IsShowingUserProperty = BindableProperty.Create(nameof(IsShowingUser), typeof(bool), typeof(WorldExplorerMap), default(bool));
	public static readonly BindableProperty PinsProperty = BindableProperty.Create(nameof(Pins), typeof(ObservableCollection<WorldExplorerPin>), typeof(WorldExplorerMap), default(ObservableCollection<WorldExplorerPin>));
	public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(WorldExplorerMap), default(DataTemplate));

	public DataTemplate ItemTemplate
	{
		get => (DataTemplate)GetValue(ItemTemplateProperty);
		set => SetValue(ItemTemplateProperty, value);
	}

	public ObservableCollection<WorldExplorerPin> Pins
	{
		get => (ObservableCollection<WorldExplorerPin>)GetValue(PinsProperty);
		set => SetValue(PinsProperty, value);
	}

	public bool IsShowingUser
	{
		get => (bool)GetValue(IsShowingUserProperty);
		set => SetValue(IsShowingUserProperty, value);
	}
}