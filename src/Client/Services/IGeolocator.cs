namespace Client.Services;

public interface IGeolocator
{
	Task StartListening(IProgress<Location> positionChangedProgress, CancellationToken cancellationToken);
}