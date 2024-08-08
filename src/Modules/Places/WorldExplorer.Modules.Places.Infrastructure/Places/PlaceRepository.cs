namespace WorldExplorer.Modules.Places.Infrastructure.Places;

using Database;
using Domain.Places;
using Microsoft.EntityFrameworkCore;

internal sealed class PlaceRepository(PlacesDbContext context) : IPlaceRepository
{
    public async Task<Place?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Places.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public void Insert(Place place)
    {
        context.Places.Add(place);
    }
}