namespace Client.Controls;

using System.Collections.ObjectModel;

public interface IArView : IView
{
	ObservableCollection<string> Images { get; }
}