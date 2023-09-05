namespace Shared.Models;

public class Visit
{
	public Guid Id { get; set; }
	public required Place Place { get; set; }

	// Visit date if you need it
	public DateTime VisitDate { get; set; }
}