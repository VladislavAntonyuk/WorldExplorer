namespace Shared.Models;

public class Review
{
	public Guid Id { get; set; }
	public int Rating { get; set; }
	public string? Comment { get; set; }
	public DateTime ReviewDate { get; set; }
}