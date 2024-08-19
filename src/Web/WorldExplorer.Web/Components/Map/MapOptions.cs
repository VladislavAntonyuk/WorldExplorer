namespace WorldExplorer.Web.Components.Map;

using Modules.Places.Application.Abstractions;
using Modules.Places.Domain.Places;

public record MapOptions(Location? Location, int Zoom)
{
	public bool TrackUserLocation { get; set; }
}