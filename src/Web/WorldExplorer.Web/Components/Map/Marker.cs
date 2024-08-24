namespace WorldExplorer.Web.Components.Map;

using Modules.Places.Application.Abstractions;

public record Marker(Location? Location, string? Title, string? Icon);