namespace WebApp.Components.Map;

using Shared.Models;

public record MapOptions(Location? Location, int Zoom)
{
	public bool TrackUserLocation { get; set; }
}