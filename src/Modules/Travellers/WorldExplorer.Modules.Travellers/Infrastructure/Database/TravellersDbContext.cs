namespace WorldExplorer.Modules.Travellers.Infrastructure.Database;

using Abstractions.Data;
using Application.Travellers;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Outbox;
using Configurations;
using Microsoft.EntityFrameworkCore;

public sealed class TravellersDbContext(DbContextOptions<TravellersDbContext> options) : DbContext(options), IUnitOfWork
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema(Schemas.Travellers);

		modelBuilder.ApplyConfiguration(new TravellerConfiguration());
		modelBuilder.ApplyConfiguration(new TravellerRouteConfiguration());

		modelBuilder.ApplyConfiguration(new PlacesConfiguration());
		modelBuilder.ApplyConfiguration(new VisitsConfiguration());
		modelBuilder.ApplyConfiguration(new ReviewsConfiguration());

		modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
		modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
		modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
		modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
	}

	internal DbSet<Traveller> Travellers => Set<Traveller>();
	internal DbSet<Visit> Visits => Set<Visit>();
	internal DbSet<Place> Places => Set<Place>();
}