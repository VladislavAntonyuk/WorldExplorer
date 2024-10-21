namespace WorldExplorer.Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequests;

using Abstractions;
using Domain.LocationInfo;

public sealed record LocationInfoRequestResponse
{
	public int Id { get; set; }
	public required Location Location { get; set; }
	public LocationInfoRequestStatus Status { get; set; }
	public DateTime CreationDate { get; set; }
}