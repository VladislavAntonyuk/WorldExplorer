namespace WorldExplorer.Client.Map.WorldExplorerMap;

using System.Windows.Input;

public class WorldExplorerPin
{
	private string? image;

	public required Guid PlaceId { get; set; }

	public string? Image
	{
		get => image ?? DefaultImage;
		set => image = value;
	}

	public required Location Location { get; set; }

	public required string Label { get; set; }

	public ICommand? MarkerClicked { get; set; }

	public static string DefaultImage { get; set; } = "https://ik.imagekit.io/VladislavAntonyuk/projects/world-explorer/default-location-pin.png";
}