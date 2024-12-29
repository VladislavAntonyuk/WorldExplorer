namespace WorldExplorer.Modules.Travellers.Application.Travellers;

using Visits;

public class Traveller
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public ICollection<Visit> Visits { get; set; } = new List<Visit>();

	public static Traveller Create(Guid travellerId, string name, ICollection<Visit> visits)
	{
		return new Traveller
		{
			Id = travellerId,
			Name = name,
			Visits = visits
		};
	}
}