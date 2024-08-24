namespace WorldExplorer.Modules.Travellers.Application.Travellers;

public class TravellerRoute
{
	public Guid Id { get; set; }
	public ICollection<Location> Locations { get; set; } = new List<Location>();
}