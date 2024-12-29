namespace WorldExplorer.Modules.Travellers.Application.Visits;

using Travellers;

public class Visit
{
	public Guid Id { get; set; }
	public required Guid TravellerId { get; set; }
	public Traveller? Traveller { get; set; }
	public required Guid PlaceId { get; set; }
	public DateTime VisitDate { get; set; }
	public Review? Review { get; set; }
}