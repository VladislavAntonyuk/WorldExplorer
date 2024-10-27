namespace Client.Controls.WorldExplorerMap;

using System.Collections.ObjectModel;

public class WorldExplorerMap : View, IWorldExplorerMap
{
	public static readonly BindableProperty UserLocationProperty = BindableProperty.Create(nameof(UserLocation), typeof(Location), typeof(WorldExplorerMap));
	public static readonly BindableProperty PinsProperty = BindableProperty.Create(nameof(Pins), typeof(ObservableCollection<WorldExplorerPin>), typeof(WorldExplorerMap));

	public ObservableCollection<WorldExplorerPin> Pins
	{
		get => (ObservableCollection<WorldExplorerPin>)GetValue(PinsProperty);
		set => SetValue(PinsProperty, value);
	}

	public Location? UserLocation
	{
		get => (Location?)GetValue(UserLocationProperty);
		set => SetValue(UserLocationProperty, value);
	}
}