namespace Client;

using System.Collections.ObjectModel;

public interface IArView : IView
{
	ObservableCollection<byte[]> Images { get; }
}