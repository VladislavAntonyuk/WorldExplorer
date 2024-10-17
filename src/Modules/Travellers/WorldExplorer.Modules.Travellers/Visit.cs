namespace WorldExplorer.Modules.Travellers;

public class Visit
{
	public Guid Id { get; set; }
	public required Guid TravellerId { get; set; }
	public required Guid PlaceId { get; set; }
	public DateTime VisitDate { get; set; }
	public Review? Review { get; set; }
}