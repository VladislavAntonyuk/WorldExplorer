namespace Client.ViewModels;

public class LocationChangedEventArgs : EventArgs
{
	public required Location Location { get; init; }
}