namespace WorldExplorer.Modules.Travellers;

using Database;
using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using Places.Domain.Places;

internal static class Tags
{
    internal const string Travellers = "Travellers";
}

public class Traveller
{
	public Guid Id { get; set; }
	public ICollection<TravellerRoute> Routes { get; set; } = new List<TravellerRoute>();
}

public class TravellerRoute
{
	public Guid Id { get; set; }
	public ICollection<Location> Locations { get; set; } = new List<Location>();
}

public class Visit
{
	public Guid Id { get; set; }
	public required Guid TravellerId { get; set; }
	public required Guid PlaceId { get; set; }
	public DateTime VisitDate { get; set; }
}

public class Place
{
	public Guid Id { get; set; }
	public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}

internal static class BrandDataLoader
{
	[DataLoader]
	public static async Task<List<Traveller>> GetTravellerByIdAsync(
		TravellersDbContext context,
		CancellationToken ct)
		=> await context.Travellers.ToListAsync();
}

public sealed class TravellersService(TravellersDbContext context)
{
	public async Task<Page<Traveller>> GetTravellersAsync(
		PagingArguments args,
		CancellationToken ct = default)
		=> await context.Travellers
		                .AsNoTracking()
		                .OrderBy(t => t.Id)
		                .ThenBy(t => t.Id)
		                .ToPageAsync(args, ct);
}