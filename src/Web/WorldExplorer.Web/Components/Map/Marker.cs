namespace WorldExplorer.Web.Components.Map;

using Modules.Places.Domain.Places;

public record Marker(Location? Location, string? Title, string? Icon);