namespace WorldExplorer.Modules.Travellers;

public class Place
{
	public Guid Id { get; set; }
	public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}