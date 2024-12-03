namespace WorldExplorer.Modules.Travellers.Application.Visits.CreateVisit;

public class VisitRequest
{
	public required Guid TravellerId { get; set; }
	public required Guid PlaceId { get; set; }
	public int Rating { get; set; }
	public string? Comment { get; set; }
}