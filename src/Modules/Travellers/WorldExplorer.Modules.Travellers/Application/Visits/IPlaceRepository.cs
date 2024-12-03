namespace WorldExplorer.Modules.Travellers.Application.Visits;

using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

public interface IPlaceRepository
{
	void Insert(Place place);
	Task DeleteAsync(CancellationToken cancellationToken);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public class PlaceRepository(TravellersDbContext dbContext) : IPlaceRepository
{
	public void Insert(Place place)
	{
		dbContext.Places.Add(place);
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		await dbContext.Places.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
	}

	public async Task DeleteAsync(CancellationToken cancellationToken)
	{
		await dbContext.Places.ExecuteDeleteAsync(cancellationToken);
	}
}