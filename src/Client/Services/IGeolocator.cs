namespace Client.Services;

using Models;

public interface IGeolocator
{
	Task StartListening(IProgress<GeolocatorData> positionChangedProgress, CancellationToken cancellationToken);
}