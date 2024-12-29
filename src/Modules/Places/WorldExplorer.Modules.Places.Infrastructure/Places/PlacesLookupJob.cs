namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Application.Abstractions;
using Application.Abstractions.Data;
using Database;
using Domain.LocationInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

[DisallowConcurrentExecution]
internal sealed class PlacesLookupJob(
	IUnitOfWork unitOfWork,
	PlacesDbContext dbContext,
	IAiService aiService,
	ILogger<PlacesLookupJob> logger) : IJob
{
	private const string ModuleName = "Places";

	public async Task Execute(IJobExecutionContext context)
	{
		logger.LogInformation("{Module} - Beginning to process locationInfoRequests messages", ModuleName);

		var locationInfoRequests = await dbContext.LocationInfoRequests
		                                          .Where(x => x.Status == LocationInfoRequestStatus.New)
		                                          .OrderBy(x => x.CreationDate)
		                                          .ToListAsync(context.CancellationToken);

		if (locationInfoRequests.Count == 0)
		{
			return;
		}

		var locationInfoRequestChunks = locationInfoRequests.Chunk(5);
		foreach (var chunk in locationInfoRequestChunks)
		{
			var locationInfoRequestIds = chunk.Select(x => x.Id).ToList();
			try
			{
				await dbContext.LocationInfoRequests.Where(x => locationInfoRequestIds.Contains(x.Id))
				               .ExecuteUpdateAsync(x => x.SetProperty(y => y.Status, LocationInfoRequestStatus.Pending),
				                                   context.CancellationToken);

				await GeneratePlaces(chunk, context.CancellationToken);

				await dbContext.LocationInfoRequests.Where(x => locationInfoRequestIds.Contains(x.Id))
				               .ExecuteUpdateAsync(
					               x => x.SetProperty(y => y.Status, LocationInfoRequestStatus.Completed),
					               context.CancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed requesting places {Requests}", string.Join(',', locationInfoRequests));
				await dbContext.LocationInfoRequests.Where(x => locationInfoRequestIds.Contains(x.Id))
				               .ExecuteUpdateAsync(x => x.SetProperty(y => y.Status, LocationInfoRequestStatus.New),
				                                   context.CancellationToken);
			}
		}

		logger.LogInformation("{Module} - Completed processing inbox messages", ModuleName);
	}

	private async Task GeneratePlaces(LocationInfoRequest[] locationInfoRequests, CancellationToken cancellationToken)
	{
		var tasks = locationInfoRequests.Select(x => aiService.GetNearByPlaces(new Location(x.Location.Y, x.Location.X), cancellationToken));
		var placeTasks = await Task.WhenAll(tasks);
		var places = placeTasks.SelectMany(x => x).ToList();

		var newPlaceNames = places.Select(x => x.Name);
		var existingPlaceNames = await dbContext.Places.Where(x => newPlaceNames.Contains(x.Name))
		                                        .Select(x => x.Name)
		                                        .ToListAsync(cancellationToken);

		var newPlaces = places.ExceptBy(existingPlaceNames, x => x.Name);
		await dbContext.Places.AddRangeAsync(newPlaces, cancellationToken);
		await unitOfWork.SaveChangesAsync(cancellationToken);
	}
}