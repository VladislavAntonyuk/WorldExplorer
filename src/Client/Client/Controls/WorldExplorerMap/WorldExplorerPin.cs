namespace Client.Controls.WorldExplorerMap;

using System.Windows.Input;

public class WorldExplorerPin
{
	public required Guid PlaceId { get; set; }

	public string? Image { get; set; }

	public required Location Location { get; set; }

	public required string Label { get; set; }

	public ICommand? MarkerClicked { get; set; }
}