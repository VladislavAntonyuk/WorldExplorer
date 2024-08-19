namespace WorldExplorer.Modules.Travellers.Application.Travellers.GetById;

using Microsoft.EntityFrameworkCore;
using Travellers;
using WorldExplorer.Modules.Travellers.Infrastructure.Database;

[ExtendObjectType("Travellers")]
public sealed class GetTravellerByIdHandler(TravellersDbContext context)
{
	public Task<Traveller?> GetById(Guid id, CancellationToken ct = default)
		=> context.Travellers
						.AsNoTracking()
						.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct);
}