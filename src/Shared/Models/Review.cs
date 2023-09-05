namespace Shared.Models;

public class Review
{
	public Guid Id { get; set; }
	public int Rating { get; set; } // You can consider it from 1-5 or 1-10
	public string? Comment { get; set; }
	public DateTime ReviewDate { get; set; }
}