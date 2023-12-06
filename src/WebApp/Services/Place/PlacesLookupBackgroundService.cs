namespace WebApp.Services.Place;

using AI;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Location = Shared.Models.Location;
using Place = Infrastructure.Entities.Place;

public class PlacesLookupBackgroundService(IDbContextFactory<WorldExplorerDbContext> dbContextFactory,
	IAiService aiService,
	ILogger<PlacesLookupBackgroundService> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await using var dbContext = await dbContextFactory.CreateDbContextAsync(stoppingToken);
			var locationInfoRequests = await dbContext.LocationInfoRequests
													  .Where(x => x.Status == LocationInfoRequestStatus.New)
													  .OrderBy(x => x.CreationDate)
													  .ToListAsync(stoppingToken);

			if (locationInfoRequests.Count == 0)
			{
				await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
			}

			var locationInfoRequestChunks = locationInfoRequests.Chunk(5);
			foreach (var chunk in locationInfoRequestChunks)
			{
				var locationInfoRequestIds = chunk.Select(x => x.Id).ToList();
				try
				{
					await dbContext.LocationInfoRequests.Where(x => locationInfoRequestIds.Contains(x.Id))
								   .ExecuteUpdateAsync(x => x.SetProperty(y => y.Status, LocationInfoRequestStatus.Pending), stoppingToken);

					await GeneratePlaces(dbContext, chunk, stoppingToken);

					await dbContext.LocationInfoRequests.Where(x => locationInfoRequestIds.Contains(x.Id))
								   .ExecuteUpdateAsync(x => x.SetProperty(y => y.Status, LocationInfoRequestStatus.Completed), stoppingToken);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Failed requesting places {Requests}", string.Join(',', locationInfoRequests));
					await dbContext.LocationInfoRequests.Where(x => locationInfoRequestIds.Contains(x.Id))
								   .ExecuteUpdateAsync(x => x.SetProperty(y => y.Status, LocationInfoRequestStatus.New), stoppingToken);

				}
			}
		}
	}

	private async Task GeneratePlaces(WorldExplorerDbContext dbContext,
		LocationInfoRequest[] locationInfoRequests,
		CancellationToken cancellationToken)
	{
		var tasks = locationInfoRequests.Select(x => aiService.GetNearByPlaces(new Location(x.Location.Y, x.Location.X)));
		var placeTasks = await Task.WhenAll(tasks);
		var places = placeTasks.SelectMany(x => x).ToList();

		var newPlaceNames = places.Select(x => x.Name);
		var existingPlaceNames = await dbContext.Places.Where(x => newPlaceNames.Contains(x.Name))
												.Select(x => x.Name)
												.ToListAsync(cancellationToken);

		var newPlaces = places.ExceptBy(existingPlaceNames, x => x.Name).Select(ToPlace);

		await dbContext.Places.AddRangeAsync(newPlaces, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
	}

	private static Place ToPlace(Shared.Models.Place place)
	{
		return new Place
		{
			Name = place.Name,
			Location = place.Location.ToPoint()
		};
	}
}