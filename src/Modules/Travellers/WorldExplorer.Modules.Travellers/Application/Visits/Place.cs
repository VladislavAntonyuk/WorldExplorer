namespace WorldExplorer.Modules.Travellers.Application.Visits;
public class Place
{
	public Guid Id { get; set; }
	public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}