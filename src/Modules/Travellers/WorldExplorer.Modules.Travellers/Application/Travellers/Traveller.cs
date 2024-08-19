namespace WorldExplorer.Modules.Travellers.Application.Travellers;
public class Traveller
{
	public Guid Id { get; set; }
	public ICollection<TravellerRoute> Routes { get; set; } = new List<TravellerRoute>();
}