namespace Client;

using Models;

public interface IGeolocator
{
	event EventHandler<GeolocatorData> PositionChanged;
	void StartListening();
	void StopListening();
}

public partial class GeolocatorImplementation : IGeolocator
{
	private readonly WeakEventManager weakEventManager = new();
	public event EventHandler<GeolocatorData> PositionChanged
	{
		add => weakEventManager.AddEventHandler(value);
		remove => weakEventManager.RemoveEventHandler(value);
	}
}