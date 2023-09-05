namespace WebApp.Infrastructure.Models;

public class Review
{
	public Guid Id { get; set; }
	public int Rating { get; set; } // You can consider it from 1-5 or 1-10
	public string? Comment { get; set; }

	// Navigation properties
	public required string UserId { get; set; }

	public Guid PlaceId { get; set; }

	// Review date if you need it
	public DateTime ReviewDate { get; set; }
}