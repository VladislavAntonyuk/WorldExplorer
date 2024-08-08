namespace WebApp.Services.Place;

//using Infrastructure;
//using Infrastructure.Entities;
//using Microsoft.EntityFrameworkCore;

//public class LocationInfoRequestsService(IDbContextFactory<WorldExplorerDbContext> dbContextFactory) : ILocationInfoRequestsService
//{
//	public async Task Clear(CancellationToken cancellationToken)
//	{
//		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
//		await dbContext.LocationInfoRequests.ExecuteDeleteAsync(cancellationToken);
//	}

//	public async Task<List<LocationInfoRequest>> GetRequests(CancellationToken cancellationToken)
//	{
//		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
//		var dbRequests = await dbContext.LocationInfoRequests.ToListAsync(cancellationToken);
//		return dbRequests;
//	}

//	public async Task Delete(int requestId, CancellationToken cancellationToken)
//	{
//		await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
//		await dbContext.LocationInfoRequests.Where(x => x.Id == requestId).ExecuteDeleteAsync(cancellationToken);
//	}
//}