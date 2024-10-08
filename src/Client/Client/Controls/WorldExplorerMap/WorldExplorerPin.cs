namespace Client.Controls.WorldExplorerMap;
public class WorldExplorerPin : View
{
	public static readonly BindableProperty LabelProperty = BindableProperty.Create(nameof(Label), typeof(string), typeof(WorldExplorerPin), default(string));
	public static readonly BindableProperty AddressProperty = BindableProperty.Create(nameof(Address), typeof(string), typeof(WorldExplorerPin), default(string));
	public static readonly BindableProperty LocationProperty = BindableProperty.Create(nameof(Location), typeof(Location), typeof(WorldExplorerPin), default(Location));
	public static readonly BindableProperty PlaceIdProperty = BindableProperty.Create(nameof(PlaceId), typeof(Guid), typeof(WorldExplorerPin), default(Guid));

	public Guid PlaceId
	{
		get => (Guid)GetValue(PlaceIdProperty);
		set => SetValue(PlaceIdProperty, value);
	}

	public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(string), typeof(WorldExplorerPin));

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

	public string Address
	{
		get => (string)GetValue(AddressProperty);
		set => SetValue(AddressProperty, value);
	}

	public string Label
	{
		get => (string)GetValue(LabelProperty);
		set => SetValue(LabelProperty, value);
	}
}