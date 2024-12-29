namespace WorldExplorer.Modules.Travellers.Infrastructure.Database;

using Abstractions.Data;
using Application.Travellers;
using Application.Visits;
using Common.Infrastructure.Inbox;
using Configurations;
using Microsoft.EntityFrameworkCore;

public sealed class TravellersDbContext(DbContextOptions<TravellersDbContext> options) : DbContext(options), IUnitOfWork
{
	internal DbSet<Traveller> Travellers => Set<Traveller>();
	internal DbSet<Visit> Visits => Set<Visit>();
	internal DbSet<Place> Places => Set<Place>();

	internal DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema(Schemas.Travellers);

		modelBuilder.ApplyConfiguration(new TravellerConfiguration());
		modelBuilder.ApplyConfiguration(new PlacesConfiguration());
		modelBuilder.ApplyConfiguration(new VisitsConfiguration());
		modelBuilder.ApplyConfiguration(new ReviewsConfiguration());

		modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
	}
}