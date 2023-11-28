namespace WebApp.Infrastructure.Entities;

using NetTopologySuite.Geometries;

public class LocationInfoRequest
{
	public int Id { get; set; }
	public required Point Location { get; set; }
	public LocationInfoRequestStatus Status { get; set; }
	public DateTime CreationDate { get; set; }
}

public enum LocationInfoRequestStatus
{
	New,
	Pending,
	Completed
}