namespace WorldExplorer.Modules.Places.Domain.LocationInfo;

using WorldExplorer.Modules.Places.Domain.Places;

public class LocationInfoRequest
{
	public int Id { get; set; }
	public required Location Location { get; set; }
	public LocationInfoRequestStatus Status { get; set; }
	public DateTime CreationDate { get; set; }
}