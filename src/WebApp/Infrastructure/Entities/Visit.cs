namespace WebApp.Infrastructure.Entities;

public class Visit
{
	public Guid Id { get; set; }
	public required string UserId { get; set; }
	public Guid PlaceId { get; set; }
	public DateTime VisitDate { get; set; }
}