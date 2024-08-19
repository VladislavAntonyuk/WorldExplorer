namespace WorldExplorer.Modules.Places.Application.Places.GetPlace;

using Abstractions;
using Domain.LocationInfo;
using Domain.Places;
using Shared.Enums;

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

public sealed record LocationInfoRequestResponse
{
	public Guid Id { get; set; }
	public required Location Location { get; set; }
	public LocationInfoRequestStatus Status { get; set; }
	public DateTime CreationDate { get; set; }
}