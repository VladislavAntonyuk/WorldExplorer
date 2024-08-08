namespace Client.Controls;

using Microsoft.Maui.Controls.Maps;

public class WorldExplorerPin : Pin
{
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
}