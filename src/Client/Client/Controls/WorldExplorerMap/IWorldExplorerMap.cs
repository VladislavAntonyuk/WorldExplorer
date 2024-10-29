namespace Client.Controls.WorldExplorerMap;

using System.Collections.ObjectModel;

public interface IWorldExplorerMap : IView
{
	ObservableCollection<WorldExplorerPin> Pins { get; }
	Location? UserLocation { get; }
	void OnMapReady();
}