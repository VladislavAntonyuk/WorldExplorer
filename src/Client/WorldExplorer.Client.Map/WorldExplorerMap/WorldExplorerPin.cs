namespace WorldExplorer.Client.Map.WorldExplorerMap;

using System.Windows.Input;

public class WorldExplorerPin : View, IWorldExplorerPin
{
	public static readonly BindableProperty MarkerClickedProperty = BindableProperty.Create(nameof(MarkerClicked), typeof(ICommand), typeof(WorldExplorerPin));

	public static readonly BindableProperty LabelProperty =
		BindableProperty.Create(nameof(Label), typeof(string), typeof(WorldExplorerPin));

	public static readonly BindableProperty LocationProperty =
		BindableProperty.Create(nameof(Location), typeof(Location), typeof(WorldExplorerPin));

	public static readonly BindableProperty PlaceIdProperty =
		BindableProperty.Create(nameof(PlaceId), typeof(Guid), typeof(WorldExplorerPin), default(Guid));

	public Guid PlaceId
	{
		get => (Guid)GetValue(PlaceIdProperty);
		set => SetValue(PlaceIdProperty, value);
	}

	public static readonly BindableProperty ImageProperty =
		BindableProperty.Create(nameof(Image), typeof(string), typeof(WorldExplorerPin));

	public string? Image
	{
		get => (string?)GetValue(ImageProperty);
		set => SetValue(ImageProperty, value);
	}

	public Location Location
	{
		get => (Location)GetValue(LocationProperty);
		set => SetValue(LocationProperty, value);
	}

	public string Label
	{
		get => (string)GetValue(LabelProperty);
		set => SetValue(LabelProperty, value);
	}

	public ICommand? MarkerClicked
	{
		get => (ICommand)GetValue(MarkerClickedProperty);
		set => SetValue(MarkerClickedProperty, value);
	}

	void IWorldExplorerPin.OnMarkerClicked()
	{
		MarkerClicked?.Execute(this);
	}
}