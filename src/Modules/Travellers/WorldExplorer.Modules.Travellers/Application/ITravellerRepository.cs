﻿namespace WorldExplorer.Modules.Travellers.Application;

using WorldExplorer.Modules.Travellers.Application.Travellers;
using WorldExplorer.Modules.Travellers.Infrastructure.Database;

public interface ITravellerRepository
{
	void Insert(Traveller traveller);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public class TravellerRepository(TravellersDbContext dbContext) : ITravellerRepository
{
	public void Insert(Traveller traveller)
	{
		dbContext.Travellers.Add(traveller);
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		
	}
}