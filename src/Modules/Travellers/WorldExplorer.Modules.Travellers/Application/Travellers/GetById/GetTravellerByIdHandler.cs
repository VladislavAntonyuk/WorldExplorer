namespace WorldExplorer.Modules.Travellers.Application.Travellers.GetById;

using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

[ExtendObjectType("Travellers")]
public sealed class GetTravellerByIdHandler(TravellersDbContext context)
{
	public Task<Traveller?> GetById(Guid id, CancellationToken ct = default)
	{
		return context.Travellers
		              .AsNoTracking()
					  .Include(t => t.Visits)
		              .ThenInclude(x => x.Review)
		              .FirstOrDefaultAsync(x => x.Id == id, ct);
	}
}