namespace WorldExplorer.Client.Map.WorldExplorerMap;

public interface IWorldExplorerPin
{
	void OnMarkerClicked();
	Guid PlaceId { get; }
	string? Image { get; }
	Location Location { get; }
	string Label { get; }
}