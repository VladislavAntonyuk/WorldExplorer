namespace Client.Models;

public class Visit
{
	public Guid Id { get; set; }
	public required Place Place { get; set; }
	public DateTime VisitDate { get; set; }
}