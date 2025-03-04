﻿namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Database;
using Domain.Places;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

internal sealed class PlaceRepository(PlacesDbContext context, IOptions<PlacesSettings> placeOptions) : IPlaceRepository
{
	public async Task<Place?> GetAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await context.Places.Include(x => x.Images).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
	}

	public async Task<List<Place>> GetAsync(CancellationToken cancellationToken = default)
	{
		return await context.Places.ToListAsync(cancellationToken);
	}

	public void Insert(Place place)
	{
		context.Places.Add(place);
	}

	public async Task<List<Place>> GetNearestPlacesAsync(Point userLocation, CancellationToken cancellationToken)
	{
		return await context.Places
							.Where(x => x.Location.IsWithinDistance(userLocation, placeOptions.Value.LocationRequestNearbyDistance))
							.ToListAsync(cancellationToken);
	}

	public void Delete(Place place)
	{
		context.Places.Remove(place);
	}

	public async Task Clear(CancellationToken cancellationToken)
	{
		await context.Places.ExecuteDeleteAsync(cancellationToken);
	}
}