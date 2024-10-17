namespace WorldExplorer.Modules.Travellers.Application.Travellers;

public class Traveller
{
	public Guid Id { get; set; }
	public ICollection<TravellerRoute> Routes { get; set; } = new List<TravellerRoute>();

	public static Traveller Create(Guid travellerId, ICollection<TravellerRoute> routes)
	{
		return new Traveller()
		{
			Id = travellerId,
			Routes = routes
		};
	}
}