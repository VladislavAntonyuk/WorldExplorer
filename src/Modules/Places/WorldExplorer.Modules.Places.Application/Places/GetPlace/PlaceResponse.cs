namespace WorldExplorer.Modules.Places.Application.Places.GetPlace;

using Abstractions;

public sealed record PlaceResponse(
	Guid Id,
	string Name,
	string? Description,
	Location Location,
	ICollection<string> Images)
{
	public string? MainImage => Images.FirstOrDefault();
}

public sealed record PlaceRequest
{
	public required string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public Location Location { get; set; } = Location.Default;
	public ICollection<string> Images { get; set; } = [];
}