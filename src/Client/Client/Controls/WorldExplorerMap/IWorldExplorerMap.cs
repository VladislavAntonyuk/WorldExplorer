namespace Client.Controls.WorldExplorerMap;

using System.Collections.ObjectModel;

public interface IWorldExplorerMap : IView
{
	ObservableCollection<WorldExplorerPin> Pins { get; }
	bool IsShowingUser { get; }
}