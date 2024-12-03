namespace WorldExplorer.Modules.Travellers.Application.Visits.CreateVisit;

using Infrastructure.Database;

public class CreateVisitHandler(TravellersDbContext context)
{
	public async Task<Visit> CreateVisit(VisitRequest request, CancellationToken ct = default)
	{
		var visit = new Visit()
		{
			Id = Guid.CreateVersion7(),
			PlaceId = request.PlaceId,
			TravellerId = request.TravellerId,
			VisitDate = DateTime.UtcNow,
			Review = new Review()
			{
				Id = Guid.CreateVersion7(),
				Comment = request.Comment,
				Rating = request.Rating
			}
		};
		context.Visits.Add(visit);
		await context.SaveChangesAsync(ct);
		return visit;
	}
}