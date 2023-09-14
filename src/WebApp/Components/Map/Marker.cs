namespace WebApp.Components.Map;

using Shared.Models;

public record Marker
{
	public Marker(MarkerOptions markerOptions)
	{
		Location = markerOptions.Location;
		Title = markerOptions.Title;
		Icon = markerOptions.Icon;
	}

	public Location? Location { get; }
	public string? Title { get; }
	public string? Icon { get; }
}