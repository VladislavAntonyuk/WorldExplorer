namespace WebApp.Infrastructure.Models;

public class Visit
{
	public Guid Id { get; set; }

	// Navigation properties
	public required string UserId { get; set; }

	public Guid PlaceId { get; set; }

	// Visit date if you need it
	public DateTime VisitDate { get; set; }
}