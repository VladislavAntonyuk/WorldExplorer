namespace WebApp.Infrastructure.Entities;

public class Review
{
	public Guid Id { get; set; }
	public int Rating { get; set; }
	public string? Comment { get; set; }
	public required string UserId { get; set; }
	public Guid PlaceId { get; set; }
	public DateTime ReviewDate { get; set; }
}