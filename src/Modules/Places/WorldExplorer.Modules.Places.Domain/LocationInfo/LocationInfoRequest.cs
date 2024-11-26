namespace WorldExplorer.Modules.Places.Domain.LocationInfo;

using NetTopologySuite.Geometries;

public class LocationInfoRequest
{
	public int Id { get; set; }
	public required Point Location { get; set; }
	public LocationInfoRequestStatus Status { get; set; }
	public DateTime CreationDate { get; set; }
}