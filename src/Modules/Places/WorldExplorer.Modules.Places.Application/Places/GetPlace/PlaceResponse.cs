namespace WorldExplorer.Modules.Places.Application.Places.GetPlace;

using Abstractions;

public sealed record PlaceResponse(
	Guid Id,
	string Name,
	string? Description,
	Location Location,
	double Rating,
	ICollection<string> Images)
{
	public string? MainImage => Images.FirstOrDefault();
}