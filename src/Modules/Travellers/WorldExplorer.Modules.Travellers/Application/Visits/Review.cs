namespace WorldExplorer.Modules.Travellers.Application.Visits;

public class Review
{
	public Guid Id { get; set; }
	public int Rating { get; set; }
	public string? Comment { get; set; }
	public Guid VisitId { get; set; }
}